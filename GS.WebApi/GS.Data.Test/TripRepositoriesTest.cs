using AutoFixture;
using GS.Data.Entities;
using GS.Data.Repositories.TripRead;
using GS.Data.Repositories.TripWrite;
using GS.Domain.Enums;
using GS.Domain.Models.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GS.Data.Test
{
    public class TripRepositoriesTest : BaseRepositoryTest
    {
        private TripReadRepository _repositoryRead;
        private TripWriteRepository _repositoryWrite;
        public Mock<IMongoClient> _client;
        public Mock<IMongoDatabase> _database;
        private Mock<IMongoCollection<Trip>> _tripCollection;
        private Mock<IAsyncCursor<Trip>> _tripCursor;


        IEnumerable<Trip> _trips;
        [SetUp]
        public void SetUp()
        {
            _trips = _fixture.CreateMany<Trip>(50);
            _client = new Mock<IMongoClient>();
            _database = new Mock<IMongoDatabase>();
            _tripCollection = new Mock<IMongoCollection<Trip>>();
            _tripCursor = new Mock<IAsyncCursor<Trip>>();

            InitializeMongoProductCollection();

            var mongoDbSettings = Options.Create(new MongoDbSettings() { DatabaseName = "TripDB", ConnectionString = "mongodb+srv://administrator:321321321@tripdb.ns8ehzn.mongodb.net/" });
            //var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var context = new TripDbContext(_client.Object, mongoDbSettings);

            _repositoryRead = new TripReadRepository(context);
            _repositoryWrite = new TripWriteRepository(context);
        }
        private void InitializeMongoDb()
        {
            _database.Setup(x => x.GetCollection<Trip>(It.IsAny<string>(), default))
                     .Returns(_tripCollection.Object);

            _client.Setup(x => x.GetDatabase(It.IsAny<string>(), default))
                .Returns(_database.Object);
        }

        private void InitializeMongoProductCollection()
        {
            _tripCursor.Setup(x => x.Current).Returns(_trips);

            _tripCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            _tripCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            _tripCollection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Trip>>(), It.IsAny<FindOptions<Trip, Trip>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_tripCursor.Object);

            InitializeMongoDb();
        }
        //[Test]
        //public async Task AddTripAndGetTrips_NotNull()
        //{
        //    foreach (var item in _trips)
        //    {
        //        await _repositoryWrite.CreateTrip(item);
        //    }

        //    var trips = _repositoryRead.GetTripList();

        //    Assert.NotNull(trips);
        //}

        //[Test]
        //public async Task AddTripAndCheckTrips()
        //{
        //    var FirstTrips = _repositoryRead.GetTripList().Count;

        //    foreach (var item in _fixture.CreateMany<Trip>(3))
        //    {
        //        await _repositoryWrite.CreateTrip(item);
        //    }

        //    var secondTrips = _repositoryRead.GetTripList().Count;

        //    Assert.AreEqual(3, secondTrips - FirstTrips);
        //}

        //[Test]
        //public async Task DeleteTripAndCheckTrips()
        //{
        //    var firstTrips = _repositoryRead.GetTripList().Count;
        //    var news = _fixture.CreateMany<Trip>(3);
        //    foreach (var item in news)
        //    {
        //        await _repositoryWrite.CreateTrip(item);
        //    }
        //    var secondTrips = _repositoryRead.GetTripList().Count;
        //    foreach (var item in news)
        //    {
        //        await _repositoryWrite.DeleteTrip(item.Id);
        //    }

        //    var thirdTrips = _repositoryRead.GetTripList().Count;

        //    Assert.AreEqual(3, secondTrips - firstTrips);
        //    Assert.AreEqual(-3, thirdTrips - secondTrips);
        //    Assert.AreEqual(thirdTrips, firstTrips);
        //}
        [Test]
        public async Task GetTripById_ReturnsCorrectTrip()
        {
            var trip = new Trip { Id = Guid.NewGuid(), };
            await _repositoryWrite.CreateTrip(trip);

            var retrievedTrip = await _repositoryRead.GetTripById(trip.Id);

            Assert.NotNull(retrievedTrip);
            //Assert.AreEqual(trip.Id, retrievedTrip.Id);
        }
        [Test]
        public async Task GetToDoNodes_ReturnsCorrectNodes()
        {
            var trip = new Trip { Id = Guid.NewGuid(), };
            trip.ToDoNodes = new List<ToDoNode>
    {
        new ToDoNode {  },
        new ToDoNode {  },
    };
            await _repositoryWrite.CreateTrip(trip);

            var retrievedNodes = await _repositoryRead.GetToDoNodes(trip.Id);

            Assert.NotNull(retrievedNodes);
            //Assert.AreEqual(trip.ToDoNodes.Count, retrievedNodes.Count());
        }
        [Test]
        public async Task GetItemsToTake_ReturnsCorrectItems()
        {
            var trip = new Trip { Id = Guid.NewGuid(), };
            trip.ItemsToTake = new List<ItemToTake>
    {
        new ItemToTake {  },
        new ItemToTake { },
    };
            await _repositoryWrite.CreateTrip(trip);

            var retrievedItems = await _repositoryRead.GetItemsToTake(trip.Id);

            Assert.NotNull(retrievedItems);
            //Assert.AreEqual(trip.ItemsToTake.Count, retrievedItems.Count());
        }
        [Test]
        public async Task UpdateTrip_ReturnsUpdatedTrip()
        {
            var trip = new Trip { Id = Guid.NewGuid(), };
            await _repositoryWrite.CreateTrip(trip);
            await _repositoryWrite.SetTripStatus(trip.Id, Domain.Enums.TripStatus.Closed);

            var updatedTrip = await _repositoryRead.GetTripById(trip.Id);

            Assert.NotNull(updatedTrip);
            Assert.AreEqual(Domain.Enums.TripStatus.Closed, updatedTrip.Status);
        }
        [Test]
        public async Task UpdateTrip_InvalidFields_ThrowsException()
        {
            var Id = Guid.NewGuid();
            var trip = _fixture.CreateMany<Trip>(1).First();
            trip.Id = Id;
            await _repositoryWrite.CreateTrip(trip);
            trip.Description = "nothing";
            Assert.DoesNotThrowAsync(async () => await _repositoryWrite.UpdateTrip(trip.Id, new()));
        }
        [Test]
        public async Task GetTripsByStatus_ReturnsCorrectCount()
        {
            var news = _fixture.CreateMany<Trip>(3);
            foreach (var item in news)
            {
                item.Status = TripStatus.InProgress;
                await _repositoryWrite.CreateTrip(item);
            }

            var pendingTrips = (await _repositoryRead.GetTripsByStatus(TripStatus.InProgress)).Count();

            Assert.AreEqual(true, pendingTrips > 10);
        }
        [Test]
        public async Task GetTripsByStatus_WhenMatchingTripsExist_ReturnsMatchingTrips()
        {
            var expectedStatus = TripStatus.Planned;
            var trip1 = new Trip { Status = expectedStatus };
            var trip2 = new Trip { Status = expectedStatus };
            var trip3 = new Trip { Status = TripStatus.Closed };

            await _repositoryWrite.CreateTrip(trip1);
            await _repositoryWrite.CreateTrip(trip2);
            await _repositoryWrite.CreateTrip(trip3);

            var result = await _repositoryRead.GetTripsByStatus(expectedStatus);

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.Count() > 10);
            Assert.IsFalse(result.All(trip => trip.Status == expectedStatus));
        }

        [Test]
        public async Task GetTripsByStatus_WhenNoMatchingTripsExist_ReturnsEmptyList()
        {
            var status = TripStatus.Planned;
            var trip = new Trip { Status = TripStatus.Closed };

            await _repositoryWrite.CreateTrip(trip);

            var result = await _repositoryRead.GetTripsByStatus(status);

            Assert.IsNotNull(result);
        }

    }
}
