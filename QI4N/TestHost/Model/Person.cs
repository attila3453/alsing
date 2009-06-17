﻿namespace ConsoleApplication23
{
    using System;
    using System.Reflection;

    using QI4N.Framework;

    [Concerns(typeof(PersonBehaviorConcern), typeof(MyGenericConcern))]
    [Mixins(typeof(PersonBehaviorMixin))]
    public interface PersonComposite : Person, Composite
    {
    }


    public interface PersonBehavior
    {
        void SayHi();
    }

    [Mixins(typeof(OinkOinkMixin))]
    public interface OinkOink
    {
        void Oink();
    }

    public class OinkOinkMixin : OinkOink
    {
        [This]
        private PersonState state;

        public void Oink()
        {
            Console.WriteLine("OinkOink {0}",state.FirstName.Value);
        }
    }

    public class PersonBehaviorMixin : PersonBehavior
    {
        [Uses]
        private string email;

        [This]
        private PersonState self;

        [This]
        private OinkOink oink;

        public void SayHi()
        {
            Console.WriteLine("{0} {1} Says hello from QI4N - email {2}", this.self.FirstName.Value, this.self.LastName.Value, this.email);

            oink.Oink();
        }
    }

    public interface Person : PersonBehavior
    {
    }

    public interface HasAddress
    {
        Property<string> Street { get; }

        Property<string> City { get; }

        Property<int> ZipCode { get; }
    }

    public interface HasName
    {
        PersonNameProperty FirstName { get; }

        PersonNameProperty LastName { get; }
    }


    public interface PersonState : HasName, HasAddress
    {
        BirthDateProperty BirthDate { get; }

        WeightProperty Weight { get; }
    }

    public interface PersonNameProperty : Property<string>
    {
    }

    public interface BirthDateProperty : Property<DateTime>
    {
    }

    public interface WeightProperty : Property<double>
    {
    }

    public class MyGenericConcern : GenericConcern
    {
        public override object Invoke(object proxy, MethodInfo method, object[] args)
        {
            Console.WriteLine("Before {0}", method.Name);
            object res = this.next.Invoke(proxy, method, args);
            Console.WriteLine("After {0}", method.Name);
            return res;
        }
    }

    public class PersonBehaviorConcern : ConcernOf<PersonBehavior>, PersonBehavior
    {
        public void SayHi()
        {
            Console.WriteLine("Before say hi");
            this.next.SayHi();
            Console.WriteLine("After say hi");
        }
    }
}