using System;
using System.Collections.Generic;
using essample.Domain;
using FluentAssertions;
using Xunit;

namespace essample.Infra.Test
{
    public class BookingTests : Spec<BookingCommand, Booking, BookingEvent>
    {
        public BookingTests() : 
            base(new Handler<BookingCommand, Booking, BookingEvent>(
                Booking.Initial,
                BookingBuilder.Create(),
                BookingDecider.Create(),
                new InMemoryStore(),
                BookingEvent.EventParser))
        {
        }

        [Fact]
        public void CreateBooking_Returns_RoomBooked_OnSuccess()
        {
            var stayPeriod = StayPeriod.FromDateTime(
                    DateTime.Now,
                    DateTime.Now.AddDays(1));
            var price = Money.FromCurrency(
                    1000,
                    "EUR");
            var roomId = new RoomId("roomId");
            Given(new List<BookingEvent> {});
            When(new Book(
                "bookingId",
                roomId,
                "guestId",
                stayPeriod,
                price));
            Then(new List<BookingEvent> {
                new RoomBooked(
                    "bookingId",
                    "guestId",
                    roomId,
                    stayPeriod,
                    price
                )
            });
            ThenState(new Booking(
                "bookingId",
                "guestId",
                roomId,
                stayPeriod,
                price,
                price,
                false
            ));
        }

        [Fact]
        public void CreateBooking_Fails_If_Booking_Exists()
        {
            var stayPeriod = StayPeriod.FromDateTime(
                    DateTime.Now,
                    DateTime.Now.AddDays(1));
            var price = Money.FromCurrency(
                    1000,
                    "EUR");
            var roomId = new RoomId("roomId");
            Action action = () => {
                Given(new List<BookingEvent> {
                    new RoomBooked(
                        "bookingId",
                        "guestId",
                        roomId,
                        stayPeriod,
                        price
                    )
                });
                When(new Book(
                    "bookingId",
                    roomId,
                    "guestId",
                    stayPeriod,
                    price)
                );
            };
            action.Should().Throw<DuplicateBookingException>();
        }
    }
}
