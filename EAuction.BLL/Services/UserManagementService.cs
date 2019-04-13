using EAuction.BLL.ExternalModels;
using EAuction.BLL.ViewModels;
using EAuction.Core.DataModels;
using EAuction.Infrastructure;
using System;
using System.Collections.Generic;
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


        public bool TwoFactorAuthentication(string userId, string userPhone)
        {
            //пользователь уже зашел-> 2FA с отправкой СМС




            return true;
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

        public UserManagementService()
        {
            _identityDbContext = new IdentityDbContext();
        }
    }
}