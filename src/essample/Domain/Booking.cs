using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace essample.Domain
{
    public abstract record BookingCommand {}
    public abstract record BookingEvent {
        public static BookingEvent EventParser(string eventType, string jsonData)
        {
            switch(eventType) {
                case "RoomBooked":
                    return JsonSerializer.Deserialize<RoomBooked>(jsonData);
                default: 
                    throw new ArgumentException("Invalid event type");
            }
        }
    }
    public record Book(
        string         BookingId,
        RoomId         RoomId,
        string         GuestId,
        StayPeriod StayPeriod,
        Money          Price
    ) : BookingCommand;

    public record RecordPayment(
        string         BookingId,
        string         PaymentId,
        float          Amount,
        string         Currency,
        string         Provider,
        DateTimeOffset PaidAt
    ) : BookingCommand;

    public record ApplyDiscount(
        string BookingId,
        float  Amount,
        string Currency
    ) : BookingCommand;

    public record RoomBooked (
        string BookingId,
        string GuestId,
        RoomId RoomId,
        StayPeriod Period,
        Money Price
    ) : BookingEvent;


    public record RoomId(string Id);

    public record Booking(
        string BookingId,
        string GuestId,
        RoomId RoomId,
        StayPeriod Period,
        Money Price,
        Money Outstanding,
        bool Paid
    ) {
        public static Booking Initial = new Booking(
            "",
            "",
            new RoomId(""),
            StayPeriod.Default,
            Money.Default,
            Money.Default,
            false
            );
    };

    public class DuplicateBookingException : Exception {
        public DuplicateBookingException(string bookingId)
        {
            BookingId = bookingId;
        }

        public string BookingId { get; }

        // override object.Equals
        public override bool Equals(object obj)
        {            
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            // TODO: write your implementation of Equals() here
            return this.BookingId == ((DuplicateBookingException)obj).BookingId;
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return BookingId.GetHashCode();
        }
    }
    public static class BookingDecider {
        public static ReadOnlyCollection<BookingEvent> Handle(Book command, Booking state)
        {
            if(state.BookingId != "") {
                throw new DuplicateBookingException(command.BookingId);
            }
            return new List<BookingEvent> { 
                new RoomBooked(
                    command.BookingId, 
                    command.GuestId,
                    command.RoomId, 
                    command.StayPeriod,
                    command.Price)
            }.AsReadOnly();
        }

        public static Func<BookingCommand, Booking, ReadOnlyCollection<BookingEvent>> Create() {
            return (command, state) => {
                switch(command)
                {
                    case Book cmd:
                        return Handle(cmd, state);
                    // case UpdateTemplateFolder updateTemplateFolder:
                    //     return Handle(updateTemplateFolder, state);
                    default:
                        throw new NotImplementedException($"Invalid command {command.GetType().FullName}");
                };
            };
        }
    }

    public static class BookingBuilder {
        public static Booking Apply(RoomBooked @event, Booking state)
        {
            return state with { 
                BookingId = @event.BookingId,
                GuestId = @event.GuestId,
                Outstanding = @event.Price,
                Paid = false,
                Period = @event.Period,
                Price = @event.Price,
                RoomId = @event.RoomId
            };
        }

        public static Booking Build(Booking state, BookingEvent @event)
        {
            switch(@event) {
                case RoomBooked evt:
                    return Apply(evt, state);
                default:
                    throw new NotImplementedException($"Invalid event {@event.GetType().FullName}");
            }            
        }

        public static Func<Booking, ReadOnlyCollection<BookingEvent>, Booking> Create() {
            return (state, events) => {
                return events.Aggregate(state, Build);
            };
        }
    }
}