using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;
using System;
using XUnit.Test.Constant;
using Multiblog.Core.Model.User;
using Multiblog.Core.Repository.Model;
using Multiblog.Core.Model.Utils;
using Multiblog.Core.Repository;

namespace XUnit.Test.Repository
{
    public class UserTestCollection : IDisposable
    {
        private MongoDBContext _context = new MongoDBContext(DBSetting.Value.Value);

        public UserTestCollection()
        {
            _context.Database.DropCollection("UserEntity");

            UserEntity[] userArray = new UserEntity[]
            {
                new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fd7"),
                    FirstName = "Thomas",
                    LastName = "Larsson",
                    Email = "ThomasLarsson@dayrep.com",
                    EmailVerified = true,
                    City = "Malå",
                    DateOfBirth = new DateTime(1988, 4, 26),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(65.104368, 18.780689))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fd8"),
                    FirstName = "Danny",
                    LastName = "Sandberg",
                    Email = "DannySandberg@rhyta.com",
                    EmailVerified = true,
                    City = "Hästveda",
                    DateOfBirth = new DateTime(1953, 4, 17),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(56.310405, 14.005946))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fd9"),
                    FirstName = "Mariam",
                    LastName = "Bergman",
                    Email = "MariamBergman@rhyta.com",
                    EmailVerified = true,
                    City = "Värnamo",
                    DateOfBirth = new DateTime(1964, 3, 2),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(57.177718, 13.980382))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fda"),
                    FirstName = "Jeanette",
                    LastName = "Forsberg",
                    Email = "JeanetteForsberg@dayrep.com",
                    EmailVerified = true,
                    City = "Krokom",
                    DateOfBirth = new DateTime(1976, 7, 31),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(63.227084, 14.538))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fdb"),
                    FirstName = "Anthon",
                    LastName = "Sandström",
                    Email = "AnthonSandstrom@cuvox.de",
                    EmailVerified = false,
                    City = "Gusum",
                    DateOfBirth = new DateTime(1986, 2, 2),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(58.349946, 16.527313))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fdc"),
                    FirstName = "Sanne",
                    LastName = "Fredriksson",
                    Email = "SanneFredriksson@armyspy.com",
                    EmailVerified = false,
                    City = "Klippan",
                    DateOfBirth = new DateTime(1994, 1, 29),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(56.063815, 13.038191))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fdd"),
                    FirstName = "Petra",
                    LastName = "Svensson",
                    Email = "PetraSvensson@jourrapide.com",
                    EmailVerified = false,
                    City = "Mörsil",
                    DateOfBirth = new DateTime(1987, 6, 18),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(63.250145, 13.570122))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fde"),
                    FirstName = "Alisa",
                    LastName = "Sandberg",
                    Email = "AlisaSandberg@rhyta.com",
                    EmailVerified = true,
                    City = "Borgafjäll",
                    DateOfBirth = new DateTime(1973, 10, 6),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(58.255314, 13.173599))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fdf"),
                    FirstName = "Nour",
                    LastName = "Ivarsson",
                    Email = "NourIvarsson@fleckens.hu",
                    EmailVerified = true,
                    City = "Romakloster",
                    DateOfBirth = new DateTime(1965, 6, 19),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(57.595642, 18.40144))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fe0"),
                    FirstName = "Anders",
                    LastName = "Olsson",
                    Email = "AndersOlsson@armyspy.com",
                    EmailVerified = true,
                    City = "Tommarp",
                    DateOfBirth = new DateTime(1973, 1, 4),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(55.450108, 14.176638))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fe1"),
                    FirstName = "Havanna",
                    LastName = "Nyman",
                    Email = "HavannaNyman@dayrep.com",
                    EmailVerified = true,
                    City = "Ullared",
                    DateOfBirth = new DateTime(1961, 5, 14),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(57.090753, 12.756162))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fe2"),
                    FirstName = "Morris",
                    LastName = "Andersson",
                    Email = "MorrisAndersson@jourrapide.com",
                    EmailVerified = true,
                    City = "Kalvträsk",
                    DateOfBirth = new DateTime(1976, 7, 6),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(65.441274, 18.910866))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fe3"),
                    FirstName = "Stella",
                    LastName = "Holmberg",
                    Email = "StellaHolmberg@gustr.com",
                    EmailVerified = true,
                    City = "Nyhammar",
                    DateOfBirth = new DateTime(1974, 12, 22),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(60.352036, 14.998837))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fe4"),
                    FirstName = "Herman",
                    LastName = "Henriksson",
                    Email = "HermanHenriksson@dayrep.com",
                    EmailVerified = true,
                    City = "Östhammar",
                    DateOfBirth = new DateTime(1972, 11, 4),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(62.683247, 17.856905))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fe5"),
                    FirstName = "Rania",
                    LastName = "Berg",
                    Email = "RaniaBerg@gustr.com",
                    EmailVerified = true,
                    City = "Kiruna",
                    DateOfBirth = new DateTime(1967, 1, 10),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(67.909934, 20.192416))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fe6"),
                    FirstName = "Björn",
                    LastName = "Ekström",
                    Email = "BjornEkstrom@dayrep.com",
                    EmailVerified = true,
                    City = "Molkom",
                    DateOfBirth = new DateTime(1968, 12, 22),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(59.664595, 13.611516))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc046063a4faca13fe7"),
                    FirstName = "Adel",
                    LastName = "Håkansson",
                    Email = "AdelHakansson@rhyta.com",
                    EmailVerified = true,
                    City = "Ramdala",
                    DateOfBirth = new DateTime(1956, 4, 27),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(56.184707, 15.771915))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13fe8"),
                    FirstName = "Mira",
                    LastName = "Hermansson",
                    Email = "MiraHermansson@rhyta.com",
                    EmailVerified = true,
                    City = "Nusnäs",
                    DateOfBirth = new DateTime(1976, 11, 3),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(60.97539, 14.580161))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13fe9"),
                    FirstName = "Ali",
                    LastName = "Jonsson",
                    Email = "AliJonsson@teleworm.us",
                    EmailVerified = true,
                    City = "Målsryd",
                    DateOfBirth = new DateTime(1955, 3, 19),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(57.87726, 14.146121))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13fea"),
                    FirstName = "Andy",
                    LastName = "Göransson",
                    Email = "AndyGoransson@dayrep.com",
                    EmailVerified = true,
                    City = "Skelleftehamn",
                    DateOfBirth = new DateTime(1957, 6, 7),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(64.697956, 21.191727))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13feb"),
                    FirstName = "Svea",
                    LastName = "Bergström",
                    Email = "SveaBergstrom@dayrep.com",
                    EmailVerified = true,
                    City = "Åkers styckebruk",
                    DateOfBirth = new DateTime(1960, 1, 15),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(59.252348, 17.018287))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13fec"),
                    FirstName = "Enes",
                    LastName = "Berggren",
                    Email = "EnesBerggren@cuvox.de",
                    EmailVerified = true,
                    City = "Trosa",
                    DateOfBirth = new DateTime(1977, 2, 26),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(58.886724, 17.459581))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13fed"),
                    FirstName = "Doris",
                    LastName = "Ek",
                    Email = "DorisEk@rhyta.com",
                    EmailVerified = true,
                    City = "Sveg",
                    DateOfBirth = new DateTime(1976, 3, 29),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(62.036606, 14.367532))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13fee"),
                    FirstName = "Marius",
                    LastName = "Sandberg",
                    Email = "MariusSandberg@einrot.com",
                    EmailVerified = true,
                    City = "Tännäs",
                    DateOfBirth = new DateTime(1980, 10, 13),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(62.492126, 12.762937))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13fef"),
                    FirstName = "Melisa",
                    LastName = "Sundberg",
                    Email = "MelisaSundberg@teleworm.us",
                    EmailVerified = true,
                    City = "Visingsö",
                    DateOfBirth = new DateTime(1980, 7, 10),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(57.987618, 14.395782))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ff0"),
                    FirstName = "Simon",
                    LastName = "Jonsson",
                    Email = "SimonJonsson@dayrep.com",
                    EmailVerified = true,
                    City = "Gräsmark",
                    DateOfBirth = new DateTime(1974, 1, 26),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(64.667693, 20.847357))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ff1"),
                    FirstName = "Elliott",
                    LastName = "Johnsson",
                    Email = "ElliottJohnsson@superrito.com",
                    EmailVerified = true,
                    City = "Vindelgransele",
                    DateOfBirth = new DateTime(1993, 11, 19),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(65.195899, 18.172658))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ff2"),
                    FirstName = "Tilda",
                    LastName = "Söderström",
                    Email = "TildaSoderstrom@superrito.com",
                    EmailVerified = true,
                    City = "Norsborg",
                    DateOfBirth = new DateTime(1982, 1, 17),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(59.27285, 17.69934))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ff3"),
                    FirstName = "Pauline",
                    LastName = "Lund",
                    Email = "PaulineLund@superrito.com",
                    EmailVerified = true,
                    City = "Asmundtorp",
                    DateOfBirth = new DateTime(1992, 8, 29),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(55.921391, 12.86538))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ff4"),
                    FirstName = "Pia",
                    LastName = "Forsberg",
                    Email = "PiaForsberg@jourrapide.com",
                    EmailVerified = true,
                    City = "Harads",
                    DateOfBirth = new DateTime(1972, 2, 20),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(66.087181, 20.944169))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ff5"),
                    FirstName = "Knut",
                    LastName = "Ek",
                    Email = "KnutEk@dayrep.com",
                    EmailVerified = true,
                    City = "Rönnäng",
                    DateOfBirth = new DateTime(1988, 7, 12),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(57.858496, 11.600414))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ff6"),
                    FirstName = "Roberto",
                    LastName = "Jönsson",
                    Email = "RobertoJonsson@gustr.com",
                    EmailVerified = true,
                    City = "Älekulla",
                    DateOfBirth = new DateTime(1968, 6, 29),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(57.683233, 13.063091))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ff7"),
                    FirstName = "Signe",
                    LastName = "Blomqvist",
                    Email = "SigneBlomqvist@superrito.com",
                    EmailVerified = true,
                    City = "Venjan",
                    DateOfBirth = new DateTime(1967, 5, 21),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(60.930631, 13.953649))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ff8"),
                    FirstName = "Erik",
                    LastName = "Berg",
                    Email = "ErikBerg@rhyta.com",
                    EmailVerified = true,
                    City = "Skog",
                    DateOfBirth = new DateTime(1971, 11, 24),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(61.186927, 16.737974))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ff9"),
                    FirstName = "Donia",
                    LastName = "Björklund",
                    Email = "DoniaBjorklund@fleckens.hu",
                    EmailVerified = true,
                    City = "Töcksfors",
                    DateOfBirth = new DateTime(1981, 5, 19),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(59.465766, 11.825412))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ffa"),
                    FirstName = "Jolin",
                    LastName = "Bergman",
                    Email = "JolinBergman@jourrapide.com",
                    EmailVerified = true,
                    City = "Junosuando",
                    DateOfBirth = new DateTime(1984, 9, 25),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(67.360133, 22.522438))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ffb"),
                    FirstName = "Isak",
                    LastName = "Larsson",
                    Email = "IsakLarsson@cuvox.de",
                    EmailVerified = true,
                    City = "Sjulsmark",
                    DateOfBirth = new DateTime(1974, 2, 23),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(65.54733, 21.563007))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ffc"),
                    FirstName = "Ninni",
                    LastName = "Abrahamsson",
                    Email = "NinniAbrahamsson@gustr.com",
                    EmailVerified = true,
                    City = "Koppom",
                    DateOfBirth = new DateTime(1978, 4, 1),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(59.80401, 12.134354))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ffd"),
                    FirstName = "Walter",
                    LastName = "Ström",
                    Email = "WalterStrom@cuvox.de",
                    EmailVerified = true,
                    City = "Tygelsjö",
                    DateOfBirth = new DateTime(1995, 8, 31),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(55.430007, 12.929217))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13ffe"),
                    FirstName = "Muhammed",
                    LastName = "Holmgren",
                    Email = "MuhammedHolmgren@superrito.com",
                    EmailVerified = true,
                    City = "Tandsbyn",
                    DateOfBirth = new DateTime(1968, 10, 25),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(62.983713, 14.803677))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca13fff"),
                    FirstName = "Lova",
                    LastName = "Jönsson",
                    Email = "LovaJonsson@superrito.com",
                    EmailVerified = true,
                    City = "Undrom",
                    DateOfBirth = new DateTime(1987, 11, 16),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(63.164077, 17.751289))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca14000"),
                    FirstName = "Jill",
                    LastName = "Eklund",
                    Email = "JillEklund@rhyta.com",
                    EmailVerified = true,
                    City = "Söderåkra",
                    DateOfBirth = new DateTime(1969, 1, 30),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(57.359163, 15.136038))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca14001"),
                    FirstName = "Yahya",
                    LastName = "Holmgren",
                    Email = "YahyaHolmgren@superrito.com",
                    EmailVerified = true,
                    City = "Särö",
                    DateOfBirth = new DateTime(1994, 1, 2),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(57.413063, 11.849666))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca14002"),
                    FirstName = "Frode",
                    LastName = "Lundqvist",
                    Email = "FrodeLundqvist@armyspy.com",
                    EmailVerified = true,
                    City = "Hälleforsnås",
                    DateOfBirth = new DateTime(1959, 10, 29),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(59.071302, 16.596933))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca14003"),
                    FirstName = "Vincent",
                    LastName = "Löfgren",
                    Email = "VincentLofgren@teleworm.us",
                    EmailVerified = true,
                    City = "Arjeplog",
                    DateOfBirth = new DateTime(1956, 2, 17),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(66.058842, 17.901436))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca14004"),
                    FirstName = "Fred",
                    LastName = "Karlsson",
                    Email = "FredKarlsson@superrito.com",
                    EmailVerified = true,
                    City = "Skurup",
                    DateOfBirth = new DateTime(1991, 1, 21),
                    Gender = Gender.Male,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(55.428826, 13.478436))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc146063a4faca14005"),
                    FirstName = "Leona",
                    LastName = "Ström",
                    Email = "LeonaStrom@dayrep.com",
                    EmailVerified = true,
                    City = "Bromölla",
                    DateOfBirth = new DateTime(1981, 12, 5),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(56.151491, 14.565161))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc246063a4faca14006"),
                    FirstName = "Jolin",
                    LastName = "Ekström",
                    Email = "JolinEkstrom@cuvox.de",
                    EmailVerified = true,
                    City = "Frövi",
                    DateOfBirth = new DateTime(1961, 10, 24),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(58.563079, 13.852404))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc246063a4faca14007"),
                    FirstName = "Elvira",
                    LastName = "Svensson",
                    Email = "ElviraSvensson@cuvox.de",
                    EmailVerified = true,
                    City = "Fjälkinge",
                    DateOfBirth = new DateTime(1987, 9, 23),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(56.096189, 14.252059))
                },
				new UserEntity()
                {
                    Id = new ObjectId("5a465bc246063a4faca14008"),
                    FirstName = "Lava",
                    LastName = "Söderberg",
                    Email = "LavaSoderberg@einrot.com",
                    EmailVerified = true,
                    City = "Almunge",
                    DateOfBirth = new DateTime(1994, 8, 1),
                    Gender = Gender.Female,
                    Headline = "Photo is my life",
                    Summary = "Taking pictures is a part of my life.All the photos I post here are taken by myself.All rights reserved.",
                    GPSPosition = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(59.852422, 17.990593))
                },

            };

            _context.UserEntityCollection.InsertMany(userArray);
        }

        public void Dispose()
        {
            _context.Database.DropCollection("UserEntity");
        }
    }
}