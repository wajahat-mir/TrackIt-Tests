using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using TrackIt.Controllers;
using TrackIt.Models;
using FluentAssertions;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TrackIt_Tests
{
	public class InventoryControllerUnitTests
	{
		[Fact]
		public async Task Items_Get_All_WithNoData()
		{
            var options = new DbContextOptionsBuilder<InventoryContext>()
                .UseInMemoryDatabase(databaseName: "Get__all_items_fail")
                .Options;

            using (var context = new InventoryContext(options))
            {
                var controller = new InventoryController(context);
                var result = await controller.GetInventoryItems();

                var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
                var items = okResult.Value.Should().BeAssignableTo<IEnumerable<InventoryItem>>().Subject;
                items.FirstOrDefault().Should().BeNull();
            }
        }

		[Fact]
		public async Task Items_Get_All_WithSeededData()
		{
			var options = new DbContextOptionsBuilder<InventoryContext>()
				.UseInMemoryDatabase(databaseName: "Get__all_items_pass")
				.Options;

			var inventoryitem = new InventoryItem() { ItemName = "Test_Name", Cost = 9.99, Quantity = 99 ,
				Dimension = new Dimension{length = 1,width = 2,depth = 3,units = "cm"},
				Brand = new Brand{Name = "ToastMaster",ContactPhone = "1234567980",
					CompanyAddress = new Address{AddressLine1 = "123 Alphabet Street",State = "NY",PostalCode = "12345",City = "New York",Country = "USA"}
				}};
			using (var context = new InventoryContext(options))
			{
				context.InventoryItems.Add(inventoryitem);
				context.SaveChanges();

				var controller = new InventoryController(context);
				var result = await controller.GetInventoryItems();

				var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
				var items = okResult.Value.Should().BeAssignableTo<IEnumerable<InventoryItem>>().Subject;
				items.Count().Should().Be(1);
			}
		}

		[Fact]
		public async Task Item_Get_ById_DoesNotExist()
		{
			var options = new DbContextOptionsBuilder<InventoryContext>()
				.UseInMemoryDatabase(databaseName: "Get__byid_fail")
				.Options;

			var inventoryitem = new InventoryItem()
			{
				Id = 1, ItemName = "Test_Name",	Cost = 9.99, Quantity = 99,
				Dimension = new Dimension { length = 1, width = 2, depth = 3, units = "cm" },
				Brand = new Brand
				{
					Name = "ToastMaster",ContactPhone = "1234567980",
					CompanyAddress = new Address { AddressLine1 = "123 Alphabet Street", State = "NY", PostalCode = "12345", City = "New York", Country = "USA" }
				}
			};
			using (var context = new InventoryContext(options))
			{
				context.InventoryItems.Add(inventoryitem);
				context.SaveChanges();

				var controller = new InventoryController(context);
				var result = await controller.GetInventoryItem(999);

				var okResult = result.Should().BeOfType<NotFoundResult>().Subject;
			}
		}

		[Fact]
		public async Task Item_Get_ById()
		{
			var options = new DbContextOptionsBuilder<InventoryContext>()
				.UseInMemoryDatabase(databaseName: "Get__byid_pass")
				.Options;

			var inventoryitem = new InventoryItem()
			{
				Id = 1,	ItemName = "Test_Name",	Cost = 9.99, Quantity = 99,
				Dimension = new Dimension { length = 1, width = 2, depth = 3, units = "cm" },
				Brand = new Brand
				{
					Name = "ToastMaster", ContactPhone = "1234567980",
					CompanyAddress = new Address { AddressLine1 = "123 Alphabet Street", State = "NY", PostalCode = "12345", City = "New York", Country = "USA" }
				}
			};
			using (var context = new InventoryContext(options))
			{
				context.InventoryItems.Add(inventoryitem);
				context.SaveChanges();

				var controller = new InventoryController(context);
				var result = await controller.GetInventoryItem(1);

				var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
                var item = okResult.Value.Should().BeAssignableTo<InventoryItem>().Subject;
                item.ItemName.Should().Be("Test_Name");
			}
		}

		[Fact]
		public async Task Item_Get_ByName_DoesNotExist()
		{
			var options = new DbContextOptionsBuilder<InventoryContext>()
				.UseInMemoryDatabase(databaseName: "Get__byName_fail")
				.Options;

			var inventoryitem = new InventoryItem()
			{
				Id = 1,
				ItemName = "Test_Name",	Cost = 9.99, Quantity = 99,
				Dimension = new Dimension { length = 1, width = 2, depth = 3, units = "cm" },
				Brand = new Brand
				{
					Name = "ToastMaster", ContactPhone = "1234567980",
					CompanyAddress = new Address { AddressLine1 = "123 Alphabet Street", State = "NY", PostalCode = "12345", City = "New York", Country = "USA" }
				}
			};
			using (var context = new InventoryContext(options))
			{
				context.InventoryItems.Add(inventoryitem);
				context.SaveChanges();

				var controller = new InventoryController(context);
				var result = await controller.GetInventoryItemByNameAsync("DoesNotExist");

				var okResult = result.Should().BeOfType<NotFoundResult>().Subject;
			}
		}

		[Fact]
		public async Task Item_Get_ByName()
		{
			var options = new DbContextOptionsBuilder<InventoryContext>()
				.UseInMemoryDatabase(databaseName: "Get__byName_pass")
				.Options;

			var inventoryitem = new InventoryItem()
			{
				Id = 1,	ItemName = "Test_Name",	Cost = 9.99,Quantity = 99,
				Dimension = new Dimension { length = 1, width = 2, depth = 3, units = "cm" },
				Brand = new Brand
				{
					Name = "ToastMaster", ContactPhone = "1234567980",
					CompanyAddress = new Address { AddressLine1 = "123 Alphabet Street", State = "NY", PostalCode = "12345", City = "New York", Country = "USA" }
				}
			};
			using (var context = new InventoryContext(options))
			{
				context.InventoryItems.Add(inventoryitem);
				context.SaveChanges();

				var controller = new InventoryController(context);
				var result = await controller.GetInventoryItemByNameAsync("Test_Name");

				var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
                var item = okResult.Value.Should().BeAssignableTo<InventoryItem>().Subject;
                item.ItemName.Should().Be("Test_Name");
            }
		}

        [Fact]
        public async Task Item_Post()
        {
            var options = new DbContextOptionsBuilder<InventoryContext>()
                .UseInMemoryDatabase(databaseName: "Post_Test_Success")
                .Options;

            var inventoryItemToPost = new InventoryItem()
            {
                Id = 1,ItemName = "PostTest",Cost = 9.99,Quantity = 99,
                Dimension = new Dimension { length = 1, width = 2, depth = 3, units = "cm" },
                Brand = new Brand
                {
                    Name = "ToastMaster",ContactPhone = "1234567980",
                    CompanyAddress = new Address { AddressLine1 = "123 Alphabet Street", State = "NY", PostalCode = "12345", City = "New York", Country = "USA" }
                }
            };

            using (var context = new InventoryContext(options))
            {
                var controller = new InventoryController(context);
                var result = await controller.PostInventoryItem(inventoryItemToPost);

                // Assert
                var Result = result.Should().BeOfType<CreatedAtActionResult>().Subject;
                var item = Result.Value.Should().BeAssignableTo<InventoryItem>().Subject;
                item.Id.Should().Be(1);
            }
        }

        [Fact]
        public async Task Item_Put_IncorrectId()
        {
            var options = new DbContextOptionsBuilder<InventoryContext>()
                .UseInMemoryDatabase(databaseName: "Put_Test_Fail")
                .Options;

            var inventoryitem = new InventoryItem()
            {
                Id = 1,
                ItemName = "Test_Name",
                Cost = 9.99,
                Quantity = 99,
                Dimension = new Dimension { length = 1, width = 2, depth = 3, units = "cm" },
                Brand = new Brand
                {
                    Name = "ToastMaster",
                    ContactPhone = "1234567980",
                    CompanyAddress = new Address { AddressLine1 = "123 Alphabet Street", State = "NY", PostalCode = "12345", City = "New York", Country = "USA" }
                }
            };
            using (var context = new InventoryContext(options))
            {
                context.InventoryItems.Add(inventoryitem);
                context.SaveChanges();

                var inventoryItemToPost = new InventoryItem()
                {
                    Id = 1,
                    ItemName = "PostTest_Updated",
                    Cost = 9.99,
                    Quantity = 99,
                    Dimension = new Dimension { length = 1, width = 2, depth = 3, units = "cm" },
                    Brand = new Brand
                    {
                        Name = "ToastMaster",
                        ContactPhone = "1234567980",
                        CompanyAddress = new Address { AddressLine1 = "123 Alphabet Street", State = "NY", PostalCode = "12345", City = "New York", Country = "USA" }
                    }
                };

                var controller = new InventoryController(context);
                var result = await controller.PutInventoryItem(999, inventoryItemToPost);

                // Assert
                result.Should().BeOfType<BadRequestResult>();
            }
        }

        [Fact]
        public async Task Item_Put_Successful()
        {
            var options = new DbContextOptionsBuilder<InventoryContext>()
                .UseInMemoryDatabase(databaseName: "Put_Test")
                .Options;

            var inventoryitem = new InventoryItem()
            {
                Id = 1,
                ItemName = "Test_Name",
                Cost = 9.99,
                Quantity = 99,
                Dimension = new Dimension { length = 1, width = 2, depth = 3, units = "cm" },
                Brand = new Brand
                {
                    Name = "ToastMaster",
                    ContactPhone = "1234567980",
                    CompanyAddress = new Address { AddressLine1 = "123 Alphabet Street", State = "NY", PostalCode = "12345", City = "New York", Country = "USA" }
                }
            };
            using (var context = new InventoryContext(options))
            {
                context.InventoryItems.Add(inventoryitem);
                context.SaveChanges();
            }

            using (var context = new InventoryContext(options))
            {
                var inventoryItemToPost = new InventoryItem()
                {
                    Id = 1,
                    ItemName = "PostTest_Updated",
                    Cost = 9.99,
                    Quantity = 99,
                    Dimension = new Dimension { length = 1, width = 2, depth = 3, units = "cm" },
                    Brand = new Brand
                    {
                        Name = "ToastMaster",
                        ContactPhone = "1234567980",
                        CompanyAddress = new Address { AddressLine1 = "123 Alphabet Street", State = "NY", PostalCode = "12345", City = "New York", Country = "USA" }
                    }
                };

                var controller = new InventoryController(context);
                var result = await controller.PutInventoryItem(1, inventoryItemToPost);

                // Assert
                var okResult = result.Should().BeOfType<NoContentResult>().Subject;
                context.InventoryItems.Where(i => i.Id == 1).FirstOrDefault().ItemName.Should().Be("PostTest_Updated");
            }
        }


        [Fact]
        public async Task Item_Delete_DoesNotExist()
        {
            var options = new DbContextOptionsBuilder<InventoryContext>()
                .UseInMemoryDatabase(databaseName: "Delete_Item_Fail")
                .Options;

            using (var context = new InventoryContext(options))
            {
                var controller = new InventoryController(context);
                var result = await controller.DeleteInventoryItem(1);

                // Assert
                var NotFoundResult = result.Should().BeOfType<NotFoundResult>().Subject;
            }
        }


        [Fact]
        public async Task Item_Delete()
        {
            var options = new DbContextOptionsBuilder<InventoryContext>()
                .UseInMemoryDatabase(databaseName: "Delete_Test")
                .Options;

            var inventoryitem = new InventoryItem()
            {
                Id = 1,
                ItemName = "Test_Name",
                Cost = 9.99,
                Quantity = 99,
                Dimension = new Dimension { length = 1, width = 2, depth = 3, units = "cm" },
                Brand = new Brand
                {
                    Name = "ToastMaster",
                    ContactPhone = "1234567980",
                    CompanyAddress = new Address { AddressLine1 = "123 Alphabet Street", State = "NY", PostalCode = "12345", City = "New York", Country = "USA" }
                }
            };
            using (var context = new InventoryContext(options))
            {
                context.InventoryItems.Add(inventoryitem);
                context.SaveChanges();

                var controller = new InventoryController(context);
                var result = await controller.DeleteInventoryItem(1);

                // Assert
                var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
                var item = okResult.Value.Should().BeAssignableTo<InventoryItem>().Subject;
                item.Id.Should().Be(1);
                context.InventoryItems.Count().Should().Be(0);
            }
        }
    }
}
