using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarin.BookReader.Models.User
{
    public class Login
    {
        /**
     * _id : 57cbf0278b37eb5f05496f8b
     * nickname : ✘。
     * avatar : /avatar/eb/a0/eba095c72cc992bdea6539ce1cfd0aff
     * exp : 0
     * lv : 1
     * gender : female
     * type : normal
     */

        public UserBean user;
        /**
         * user : {"_id":"57cbf0278b37eb5f05496f8b","nickname":"✘。","avatar":"/avatar/eb/a0/eba095c72cc992bdea6539ce1cfd0aff","exp":0,"lv":1,"gender":"female","type":"normal"}
         * token : gmPcsbwQ41UfTQEc7yMnBiRY
         */

        public String token;

        public class UserBean
        {
            public String _id;
            public String nickname;
            public String avatar;
            public int exp;
            public int lv;
            public String gender;
            public String type;

            public override string ToString()
            {
                return "UserBean{" +
                        "_id='" + _id + '\'' +
                        ", nickname='" + nickname + '\'' +
                        ", avatar='" + avatar + '\'' +
                        ", exp=" + exp +
                        ", lv=" + lv +
                        ", gender='" + gender + '\'' +
                        ", type='" + type + '\'' +
                        '}';
            }
        }
    }
}
