using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace XUnit.Test
{
    public static class UserData
    {
        public static void GenerateUsers()
        {
            string[] userLine = new string[] {
"ThomasLarsson@dayrep.com,male,Thomas,Larsson,MALÅ,Oboy1988,jooshoo6aeZ9,4/26/1988,65.104368,18.780689",
"DannySandberg@rhyta.com,male,Danny,Sandberg,HÄSTVEDA,Theret,choh8Uhie,4/17/1953,56.310405,14.005946",
"MariamBergman@rhyta.com,female,Mariam,Bergman,VÄRNAMO,Embed1964,Veich0EeNei,3/2/1964,57.177718,13.980382",
"JeanetteForsberg@dayrep.com,female,Jeanette,Forsberg,KROKOM,Annimande,phiD7eitae,7/31/1976,63.227084,14.538",
"AnthonSandstrom@cuvox.de,male,Anthon,Sandström,GUSUM,Theong,vub5nix4Ai,2/2/1986,58.349946,16.527313",
"SanneFredriksson@armyspy.com,female,Sanne,Fredriksson,KLIPPAN,Trustold,Xoo7Oogie,1/29/1994,56.063815,13.038191",
"PetraSvensson@jourrapide.com,female,Petra,Svensson,MÖRSIL,Weland,weeMahp4ae,6/18/1987,63.250145,13.570122",
"AlisaSandberg@rhyta.com,female,Alisa,Sandberg,BORGAFJÄLL,Afterand,OochodeiF5Ae,10/6/1973,58.255314,13.173599",
"NourIvarsson@fleckens.hu,female,Nour,Ivarsson,ROMAKLOSTER,Lowead65,ajaiXoom1P,6/19/1965,57.595642,18.40144",
"AndersOlsson@armyspy.com,male,Anders,Olsson,TOMMARP,Gaver1973,maB6wei4aequ,1/4/1973,55.450108,14.176638",
"HavannaNyman@dayrep.com,female,Havanna,Nyman,ULLARED,Sionquitty,ahNgoori1ee,5/14/1961,57.090753,12.756162",
"MorrisAndersson@jourrapide.com,male,Morris,Andersson,KALVTRÄSK,Calle1976,Aetoovoo9w,7/6/1976,65.441274,18.910866",
"StellaHolmberg@gustr.com,female,Stella,Holmberg,NYHAMMAR,Rourad,aeph3Aig8Noo,12/22/1974,60.352036,14.998837",
"HermanHenriksson@dayrep.com,male,Herman,Henriksson,ÖSTHAMMAR,Unwhan,UeroiXoe6ai,11/4/1972,62.683247,17.856905",
"RaniaBerg@gustr.com,female,Rania,Berg,KIRUNA,Perrid,ea3Ohgu3ph,1/10/1967,67.909934,20.192416",
"BjornEkstrom@dayrep.com,male,Björn,Ekström,MOLKOM,Poseveropme,lohr6Xae,12/22/1968,59.664595,13.611516",
"AdelHakansson@rhyta.com,male,Adel,Håkansson,RAMDALA,Narce1956,Aero7chie,4/27/1956,56.184707,15.771915",
"MiraHermansson@rhyta.com,female,Mira,Hermansson,NUSNÄS,Susaing,cho1ahHie8,11/3/1976,60.97539,14.580161",
"AliJonsson@teleworm.us,male,Ali,Jonsson,MÅLSRYD,Warch1955,Aewahg5eiT7,3/19/1955,57.87726,14.146121",
"AndyGoransson@dayrep.com,male,Andy,Göransson,SKELLEFTEHAMN,Heinink,ooYoop7ohv,6/7/1957,64.697956,21.191727",
"SveaBergstrom@dayrep.com,female,Svea,Bergström,ÅKERS STYCKEBRUK,Corgentor,Phahz4quook,1/15/1960,59.252348,17.018287",
"EnesBerggren@cuvox.de,male,Enes,Berggren,TROSA,Wriand,Oqu3LeepoN2,2/26/1977,58.886724,17.459581",
"DorisEk@rhyta.com,female,Doris,Ek,SVEG,Youreforn76,vahm0uiL9soo,3/29/1976,62.036606,14.367532",
"MariusSandberg@einrot.com,male,Marius,Sandberg,TÄNNÄS,Rithatis,Oa2aath5ah,10/13/1980,62.492126,12.762937",
"MelisaSundberg@teleworm.us,female,Melisa,Sundberg,VISINGSÖ,Rets1980,Aecu5choh,7/10/1980,57.987618,14.395782",
"SimonJonsson@dayrep.com,male,Simon,Jonsson,GRÄSMARK,Squied1974,shifi4IHie,1/26/1974,64.667693,20.847357",
"ElliottJohnsson@superrito.com,male,Elliott,Johnsson,VINDELGRANSELE,Liis1993,aefoon3uYee3,11/19/1993,65.195899,18.172658",
"TildaSoderstrom@superrito.com,female,Tilda,Söderström,NORSBORG,Usad1982,eev4eeJ4z,1/17/1982,59.27285,17.69934",
"PaulineLund@superrito.com,female,Pauline,Lund,ASMUNDTORP,Beake1992,Leig2aesoo2ie,8/29/1992,55.921391,12.86538",
"PiaForsberg@jourrapide.com,female,Pia,Forsberg,HARADS,Lawyn1972,Dok4Eine,2/20/1972,66.087181,20.944169",
"KnutEk@dayrep.com,male,Knut,Ek,RÖNNÄNG,Hakis1988,aX8Fienuqu,7/12/1988,57.858496,11.600414",
"RobertoJonsson@gustr.com,male,Roberto,Jönsson,ÄLEKULLA,Gabout,maeyoK3aif,6/29/1968,57.683233,13.063091",
"SigneBlomqvist@superrito.com,female,Signe,Blomqvist,VENJAN,Welessay,mo1Aidoo8,5/21/1967,60.930631,13.953649",
"ErikBerg@rhyta.com,male,Erik,Berg,SKOG,Rusuremb,aeQuah0eeng,11/24/1971,61.186927,16.737974",
"DoniaBjorklund@fleckens.hu,female,Donia,Björklund,TÖCKSFORS,Evand1981,chaiweRiunuu8,5/19/1981,59.465766,11.825412",
"JolinBergman@jourrapide.com,female,Jolin,Bergman,JUNOSUANDO,Pladithe,jooSh3bah,9/25/1984,67.360133,22.522438",
"IsakLarsson@cuvox.de,male,Isak,Larsson,SJULSMARK,Wamill,daiGho1geir,2/23/1974,65.54733,21.563007",
"NinniAbrahamsson@gustr.com,female,Ninni,Abrahamsson,KOPPOM,Faciabove,zinah6Eejei,4/1/1978,59.80401,12.134354",
"WalterStrom@cuvox.de,male,Walter,Ström,TYGELSJÖ,Firetry,bePei6ai,8/31/1995,55.430007,12.929217",
"MuhammedHolmgren@superrito.com,male,Muhammed,Holmgren,TANDSBYN,Hishostright1968,Lah3hah4ee,10/25/1968,62.983713,14.803677",
"LovaJonsson@superrito.com,female,Lova,Jönsson,UNDROM,Buttires,Aef7thood1n,11/16/1987,63.164077,17.751289",
"JillEklund@rhyta.com,female,Jill,Eklund,SÖDERÅKRA,Expenton,eerooKeYaX7,1/30/1969,57.359163,15.136038",
"YahyaHolmgren@superrito.com,male,Yahya,Holmgren,SÄRÖ,Soory1994,shoovuCh2,1/2/1994,57.413063,11.849666",
"FrodeLundqvist@armyspy.com,male,Frode,Lundqvist,HÄLLEFORSNÅS,Lonedity,iva7aiSh1Oh,10/29/1959,59.071302,16.596933",
"VincentLofgren@teleworm.us,male,Vincent,Löfgren,ARJEPLOG,Takelp,ier4EeHiu7e,2/17/1956,66.058842,17.901436",
"FredKarlsson@superrito.com,male,Fred,Karlsson,SKURUP,Thromervair91,Peleerie0,1/21/1991,55.428826,13.478436",
"LeonaStrom@dayrep.com,female,Leona,Ström,BROMÖLLA,Dientiong,aile2eo9Je0,12/5/1981,56.151491,14.565161",
"JolinEkstrom@cuvox.de,female,Jolin,Ekström,FRÖVI,Harle1961,mooLa3xiesh,10/24/1961,58.563079,13.852404",
"ElviraSvensson@cuvox.de,female,Elvira,Svensson,FJÄLKINGE,Suicklentrot,Liequ2noo,9/23/1987,56.096189,14.252059",
"LavaSoderberg@einrot.com,female,Lava,Söderberg,ALMUNGE,Exhad1994,eghooBa3s,8/1/1994,59.852422,17.990593"
            };

            foreach (var item in userLine)
            {
                string[] items = item.Split(",");

                int[] datesNo = items[7].Split("/").Select(x => int.Parse(x)).ToArray();
                string city = items[4].ToLower();

                city = city[0].ToString().ToUpper() + new string(city.Skip(1).ToArray());
                Trace.WriteLine("\t\t\t\tnew UserEntity()");
                Trace.WriteLine("\t\t\t\t{");
                Trace.WriteLine($"\t\t\t\t    Id = new ObjectId(\"{ObjectId.GenerateNewId()}\"),");
                Trace.WriteLine($"\t\t\t\t    FirstName = \"{items[2]}\",");
                Trace.WriteLine($"\t\t\t\t    LastName = \"{items[3]}\",");
                Trace.WriteLine($"\t\t\t\t    Email = \"{items[0]}\",");
                Trace.WriteLine($"\t\t\t\t    EmailVerified = true,");
                Trace.WriteLine($"\t\t\t\t    City = \"{city}\",");
                Trace.WriteLine($"\t\t\t\t    DateOfBirth = new DateTime({datesNo[2]}, {datesNo[0]}, {datesNo[1]}),");
                Trace.WriteLine($"\t\t\t\t    Gender = Gender.{(items[1] == "male" ? "Male":"Female")},");
                Trace.WriteLine($"\t\t\t\t    Headline = \"Photo is my life\",");
                Trace.WriteLine($"\t\t\t\t    Summary = \"Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.\",");
                Trace.WriteLine($"\t\t\t\t    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates({items[8]}, {items[9]}))");
                Trace.WriteLine("\t\t\t\t},");                
            } 
        }
    }
}
