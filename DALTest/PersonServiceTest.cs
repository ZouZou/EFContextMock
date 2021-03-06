﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.DataLayer;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace DAL.Test
{
    [TestFixture]
    public class PersonServiceTest : BaseTest
    {
	    [Test]
	    public async Task Find_at_least_one_person()
	    {
			var persons = await PersonService.GetPersons("taxcode1");

		    persons.Should().NotBeNullOrEmpty();
		    persons.Count().ShouldBeEquivalentTo(1);
		}

		[Test]
		public async Task New_person_is_saved()
		{
			var person = new Person()
			{
				TaxCode = "taxcode3",
				Firstname = "firstname3",
				Surname = "surname3"
			};

			await PersonService.AddPerson(person);

			MockSet.Verify(m => m.Add(person), Times.Once);
			MockContext.Object.Persons.Count().ShouldBeEquivalentTo(3);
			MockContext.Verify(m => m.SaveChangesAsync(), Times.Once);
		}

		[Test]
		public async Task Modified_person_is_saved()
		{
			var person = new Person()
			{
				TaxCode = "taxcode1",
				Firstname = "firstname1",
				Surname = "surname1 changed"
			};

			await PersonService.UpdatePerson(person);

			MockContext.Verify(m => m.SaveChangesAsync(), Times.Once);
		}

		[Test]
		public async Task Person_is_deleted()
		{
			var person = MockContext.Object.Persons.First(p => p.TaxCode == "taxcode1");

			await PersonService.DeletePerson(person);

			MockSet.Verify(m => m.Remove(person), Times.Once);
			MockContext.Object.Persons.Count().ShouldBeEquivalentTo(1);
			MockContext.Verify(m => m.SaveChangesAsync(), Times.Once);
		}

	    [Test]
	    public async Task deletion_for_unexisting_person_throw_exception()
	    {
		    try
		    {
			    var person = MockContext.Object.Persons.First(p => p.TaxCode == "taxcode3");

			    await PersonService.DeletePerson(person);

				true.ShouldBeEquivalentTo(false);
			}
		    catch (Exception e)
		    {
			    e.Should().BeOfType<InvalidOperationException>();
		    }
		}
	}
}