using EAuction.BLL.ExternalModels;
using EAuction.BLL.ViewModels;
using EAuction.Core.DataModels;
using EAuction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace EAuction.BLL.Services
{
    public class UserManagementService
    {
        private readonly IdentityDbContext _identityDbContext;

        private bool IsValidUser(UserLogOnViewModel model)
        {
            model.GeoLocation = GeoLocationInfo.GetGeolocationInfo();

            //проверяем, есть ли емейл в базе
            var user = _identityDbContext.ApplicationUsers.Include("ApplicationUserPasswordHistories")
                .SingleOrDefault(p => p.Email == model.Email);
            if (user == null)
                throw new Exception($"Пользователя с email {model.Email} нет в базе");

            //проверяем, подходит ли пароль емейлу
            var userPassword = user.ApplicationUserPasswordHistories.SingleOrDefault(p => p.Password == model.Password);
            if (userPassword==null)
            {
                user.FailedSignInCount+=1;
                _identityDbContext.SaveChanges();
                throw new Exception("Неверный пароль");
            }
            if (userPassword!=null && userPassword.InvalidatedDate!=null)
            {
                user.FailedSignInCount += 1;
                _identityDbContext.SaveChanges();
                throw new Exception("Аккаунт пользователя заблокирован");
            }

            //добавляем строку нового входа в таблице ApplicationUserSignInHistories в БД
            ApplicationUserSignInHistory userSignInHistory = new ApplicationUserSignInHistory()
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = user.Id,
                SignInTime = DateTime.Now,
                MachineIp = model.GeoLocation.ip,
                IpToGeoCountry = model.GeoLocation.country_name,
                IpToGeoCity = model.GeoLocation.city,
                IpToGeoLatitude = model.GeoLocation.latitude,
                IpToGeoLongitude = model.GeoLocation.longitude
            };
            _identityDbContext.ApplicationUserSignInHistories.Add(userSignInHistory);
            _identityDbContext.SaveChanges();

            return true;
        }

        /// <summary>
        /// Проверка, находится ли юзер дальше 2000 км по сравнению с предыдущими 5 входами в систему
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsUserFarFromLast5SignIn(string userId) 
        {           
            var userSignInHistory = _identityDbContext.ApplicationUserSignInHistories
                .Where(p => p.ApplicationUserId.ToString() == userId).ToList();
            if (userSignInHistory.Count == 0)
                throw new Exception($"У пользователя {userId} нет истории входов в базе");

            var currentSignIn = userSignInHistory.OrderByDescending(p => p.SignInTime).SingleOrDefault();
            var userPrev5SignIn = userSignInHistory.Where(p=>p.SignInTime!= currentSignIn.SignInTime).OrderByDescending(p=>p.SignInTime).ToList();
            if (userPrev5SignIn.Count == 0)
                throw new Exception($"У пользователя {userId} нет истории предыдущих входов в базе");

            double currentLatitude = currentSignIn.IpToGeoLatitude;
            double currentLongitude = currentSignIn.IpToGeoLongitude;
            GeoCoordinate currentCoordinate = new GeoCoordinate(currentLatitude, currentLongitude);
            
            int cnt = 1;            
            foreach (ApplicationUserSignInHistory item in userPrev5SignIn)
            {
                if (cnt >= 5)
                    break;
                double latitude = item.IpToGeoLatitude;
                double longitude = item.IpToGeoLongitude;
                GeoCoordinate tmp = new GeoCoordinate(latitude, longitude);

                if (currentCoordinate.GetDistanceTo(tmp) / 1000 > 2000)
                    return true;
                cnt++;
            }

            return false;
        }


        public int SendSmsCodeToUser(string userPhone)
        {
            const string accountSid = "ACc707a32f2606933b91eea16095ac5924";
            const string authToken = "e60854654efd3a2e798455493970be9d";
            Random rnd = new Random();
            int smsCode = rnd.Next(1000, 9999);

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: $"Enter this code {smsCode}",
                from: new Twilio.Types.PhoneNumber("+12024997374"),
                to: new Twilio.Types.PhoneNumber(userPhone)
            );
            
            return smsCode;
        }

        /// <summary>
        /// Cчитаем кол-во успешных авторизаций с момента последней смены пароля
        /// </summary>
        /// <param name="userId"></param>
        public void MandatoryUserPasswordChange(string userId)
        {
            var userSignInHistory = _identityDbContext.ApplicationUserSignInHistories
               .Where(p => p.ApplicationUserId.ToString() == userId).ToList();
            if (userSignInHistory.Count == 0)
                throw new Exception($"У пользователя {userId} нет истории входов в базе");

            var currentSignIn = userSignInHistory.OrderByDescending(p => p.SignInTime).SingleOrDefault();
            var userPrevSignIn = userSignInHistory.Where(p => p.SignInTime != currentSignIn.SignInTime).OrderByDescending(p => p.SignInTime).ToList();

            
        }

        public UserManagementService()
        {
            _identityDbContext = new IdentityDbContext();
        }
    }
}