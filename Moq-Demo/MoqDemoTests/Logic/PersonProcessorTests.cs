using System;
using System.Collections.Generic;
using Autofac.Extras.Moq;
using DemoLibrary.Logic;
using DemoLibrary.Models;
using DemoLibrary.Utilities;
using Moq;
using Xunit;

namespace MoqDemoTests.Logic
{
    public class PersonProcessorTests
    {
        [Theory]
        [InlineData("6'8\"", true, 80)]
        [InlineData("6\"8'", false, 0)]
        [InlineData("six'eight\"", false, 0)]
        public void ConvertHeightTextToInches_VariousOptions(
            string heightText, 
            bool expectedIsValid, 
            double expectedHeightInInches)
        {
            PersonProcessor processor = new PersonProcessor(null);

            var actual = processor.ConvertHeightTextToInches(heightText);

            Assert.Equal(expectedIsValid, actual.isValid);
            Assert.Equal(expectedHeightInInches, actual.heightInInches);
        }

        [Theory]
        [InlineData("Andrei", "Mitrev", "6'8\"", 80)]
        [InlineData("Ivan", "Yordanov", "5'4\"", 64)]
        public void CreatePerson_Successful(string firstName, string lastName, string heightText, double expectedHeight)
        {
            PersonProcessor processor = new PersonProcessor(null);

            PersonModel expected = new PersonModel
            {
                FirstName = firstName,
                LastName = lastName,
                HeightInInches = expectedHeight,
                Id = 0
            };

            var actual = processor.CreatePerson(firstName, lastName, heightText);

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
            Assert.Equal(expected.HeightInInches, actual.HeightInInches);

        }

        [Theory]
        [InlineData("Andrei%", "Mitrev", "6'8\"", "firstName")]
        [InlineData("Ivan", "C8ey", "5'4\"", "lastName")]
        [InlineData("Simeon", "Yordanov", "SixTwo", "heightText")]
        [InlineData("", "Yordanov", "5'11\"", "firstName")]
        public void CreatePerson_ThrowsException(string firstName, string lastName, string heightText, string expectedInvalidParameter)
        {
            PersonProcessor processor = new PersonProcessor(null);

            var ex = Record.Exception(() =>processor.CreatePerson(firstName, lastName, heightText));

            Assert.NotNull(ex);
            Assert.IsType<ArgumentException>(ex);
            if (ex is ArgumentException argEx)
            {
                Assert.Equal(expectedInvalidParameter, argEx.ParamName);
            }

        }

        [Fact]
        public void LoadPeople_ValidCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<ISqliteDataAccess>()
                    .Setup(x => x.LoadData<PersonModel>("select * from Person"))
                    .Returns(GetSamplePeople());

                var cls = mock.Create<PersonProcessor>();
                var expected = GetSamplePeople();

                var actual = cls.LoadPeople();

                Assert.True(actual != null);
                Assert.Equal(expected.Count, actual.Count);

                for (int i = 0; i < expected.Count; i++)
                {
                    Assert.Equal(expected[i].FirstName, actual[i].FirstName);
                    Assert.Equal(expected[i].LastName, actual[i].LastName);
                }
            }
        }

        [Fact]
        public void SavePeople_ValidCall()
        {
            using (var mock = AutoMock.GetLoose())
            {
                var person = new PersonModel
                {
                    Id = 1,
                    FirstName = "Andrei",
                    LastName = "Mitrev",
                    HeightInInches = 80
                };
                string sql = "insert into Person (FirstName, LastName, HeightInInches) " +
                "values ('Andrei', 'Mitrev', 80)";

                mock.Mock<ISqliteDataAccess>()
                    .Setup(x => x.SaveData(person, sql));

                var cls = mock.Create<PersonProcessor>();

                cls.SavePerson(person);

                mock.Mock<ISqliteDataAccess>()
                    .Verify(x => x.SaveData(person, sql), Times.Exactly(1));
                
            }
        }

        private List<PersonModel> GetSamplePeople()
        {
            List<PersonModel> output = new List<PersonModel>
            {
                new PersonModel
                {
                    FirstName = "Andrei",
                    LastName = "Mitrev"
                },
                new PersonModel
                {
                    FirstName = "Charity",
                    LastName = "Yordanov"
                },
                new PersonModel
                {
                    FirstName = "Simeon",
                    LastName = "Yordanov"
                },
                new PersonModel
                {
                    FirstName = "Daniel",
                    LastName = "Yordanov"
                }
            };

            return output;
        }
    }
}
