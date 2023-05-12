using AutoFixture;
using FluentAssertions;
using GS.Data.Entities;
using GS.Data.Repositories.TripRead;
using GS.Domain.Enums;
using GS.Domain.Models.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GS.Data.Test
{
    public class TripReadRepositoryTest : BaseRepositoryTest
    {
        public Mock<IMongoClient> _client;
        public Mock<IMongoDatabase> _database;
        private Mock<IMongoCollection<Trip>> _tripCollection;
        private Mock<IAsyncCursor<Trip>> _tripCursor;
        private IEnumerable<Trip> _trips;

        private TripReadRepository _repositpry;

        [SetUp]
        public void SetUp()
        {
            _trips = _fixture.CreateMany<Trip>(5);

            _database = new Mock<IMongoDatabase>();
            _client = new Mock<IMongoClient>();
            _tripCollection = new Mock<IMongoCollection<Trip>>();
            _tripCursor = new Mock<IAsyncCursor<Trip>>();

            InitializeMongoProductCollection();

            var mongoDbSettings = Options.Create(new MongoDbSettings() { DatabaseName = "TripDB", ConnectionString = "mongodb+srv://administrator:321321321@tripdb.ns8ehzn.mongodb.net/" });
            var context = new TripDbContext(_client.Object, mongoDbSettings);

            _repositpry = new TripReadRepository(context);
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


        [Test]
        public async Task GetTripById_ShouldReturnTrip()
        {
            var result = await _repositpry.GetTripById(Guid.Empty);

            result.Should().NotBeNull();
        }

        [Test]
        public async Task GetItemsToTake_ShouldReturnItemsToTake_NotNull()
        {
            var itemsToTake = _trips.FirstOrDefault().ItemsToTake;

            var result = await _repositpry.GetItemsToTake(Guid.Empty);

            result.Should().NotBeNull();
        }
        [Test]
        public async Task GetItemsToTake_ShouldReturnItemsToTake_HaveCount()
        {
            var itemsToTake = _trips.FirstOrDefault().ItemsToTake;

            var result = await _repositpry.GetItemsToTake(Guid.Empty);

            result.Should().HaveCount(itemsToTake.Count);
        }

        [Test]
        public async Task GetToDoNodes_ShouldReturnToDoNodes_HaveCount()
        {
            var toDoNodes = _trips.FirstOrDefault().ToDoNodes;

            var result = await _repositpry.GetToDoNodes(Guid.Empty);

            result.Should().HaveCount(toDoNodes.Count);
        }
        [Test]
        public async Task GetToDoNodes_ShouldReturnToDoNodes_NotNull()
        {
            var toDoNodes = _trips.FirstOrDefault().ToDoNodes;

            var result = await _repositpry.GetToDoNodes(Guid.Empty);

            result.Should().NotBeNull();
        }

        [Test]
        public async Task GetUserTrips_WithValidUserId_ReturnsUserTrips_NotNull()
        {
            var userId = Guid.NewGuid();

            var result = await _repositpry.GetUserTrips(userId);

            Assert.NotNull(result);
        }
        [Test]
        public async Task GetUserTrips_WithValidUserId_ReturnsUserTrips_IsInstanceOf()
        {
            var userId = Guid.NewGuid();

            var result = await _repositpry.GetUserTrips(userId);

            Assert.IsInstanceOf<IEnumerable<Trip>>(result);
        }

        [Test]
        public async Task GetTripById_WithValidTripId_ReturnsTrip_NotNull()
        {
            var tripId = Guid.NewGuid();

            var result = await _repositpry.GetTripById(tripId);

            Assert.NotNull(result);
        }
        [Test]
        public async Task GetTripById_WithValidTripId_ReturnsTrip_IsInstanceOf()
        {
            var tripId = Guid.NewGuid();

            var result = await _repositpry.GetTripById(tripId);

            Assert.IsInstanceOf<Trip>(result);
        }

        [Test]
        public async Task GetTripByStatusPlanned_WithValidTripId_ReturnsTrip_NotNull()
        {
            var tripId = Guid.NewGuid();

            var result = await _repositpry.GetTripsByStatus(TripStatus.Planned);

            Assert.NotNull(result);
        }
        
        [Test]
        public async Task GetTripByStatusClosed_WithValidTripId_ReturnsTrip_NotNull()
        {
            var tripId = Guid.NewGuid();

            var result = await _repositpry.GetTripsByStatus(TripStatus.Closed);

            Assert.NotNull(result);
        }
        
        [Test]
        public async Task GetTripByStatusInProgress_WithValidTripId_ReturnsTrip_NotNull()
        {
            var tripId = Guid.NewGuid();

            var result = await _repositpry.GetTripsByStatus(TripStatus.InProgress);

            Assert.NotNull(result);
        }
        

        [Test]
        public async Task GetToDoNodes_WithValidTripId_ReturnsToDoNodes_NotNull()
        {
            var tripId = Guid.NewGuid();

            var result = await _repositpry.GetToDoNodes(tripId);

            Assert.NotNull(result);
        }
        [Test]
        public async Task GetToDoNodes_WithValidTripId_ReturnsToDoNodes_IsInstanceOf()
        {
            var tripId = Guid.NewGuid();

            var result = await _repositpry.GetToDoNodes(tripId);

            Assert.IsInstanceOf<IEnumerable<ToDoNode>>(result);
        }

        [Test]
        public async Task GetItemsToTake_WithValidTripId_ReturnsItemsToTake_NotNull()
        {
            var tripId = Guid.NewGuid();

            var result = await _repositpry.GetItemsToTake(tripId);

            Assert.NotNull(result);
        }
        [Test]
        public async Task GetItemsToTake_WithValidTripId_ReturnsItemsToTake_IsInstanceOf()
        {
            var tripId = Guid.NewGuid();

            var result = await _repositpry.GetItemsToTake(tripId);

            Assert.IsInstanceOf<IEnumerable<ItemToTake>>(result);
        }
    }
}
