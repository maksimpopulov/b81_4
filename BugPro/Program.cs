﻿using System;
using Stateless;

namespace BugPro
{
    public enum BugState
    {
        New,
        Triage,
        NeedInfo,
        InProgress,
        NotABug,
        Duplicate,
        CannotReproduce,
        Fixed,
        Closed,
        Reopened
    }

    public enum BugTrigger
    {
        StartTriage,
        NeedMoreInfo,
        AssignToDev,
        MarkNotABug,
        MarkDuplicate,
        MarkCannotReproduce,
        Fix,
        Verify,
        Close,
        Reopen,
        ReturnToTriage
    }

    public class Bug
    {
        private readonly StateMachine<BugState, BugTrigger> _stateMachine;

        public BugState State => _stateMachine.State;

        public Bug()
        {
            _stateMachine = new StateMachine<BugState, BugTrigger>(BugState.New);
            SetupTransitions();
        }

        private void SetupTransitions()
        {
            _stateMachine.Configure(BugState.New)
                .Permit(BugTrigger.StartTriage, BugState.Triage);

            _stateMachine.Configure(BugState.Triage)
                .Permit(BugTrigger.NeedMoreInfo, BugState.NeedInfo)
                .Permit(BugTrigger.AssignToDev, BugState.InProgress)
                .Permit(BugTrigger.MarkNotABug, BugState.NotABug)
                .Permit(BugTrigger.MarkDuplicate, BugState.Duplicate)
                .Permit(BugTrigger.MarkCannotReproduce, BugState.CannotReproduce);

            _stateMachine.Configure(BugState.NeedInfo)
                .Permit(BugTrigger.ReturnToTriage, BugState.Triage);

            _stateMachine.Configure(BugState.InProgress)
                .Permit(BugTrigger.Fix, BugState.Fixed);

            _stateMachine.Configure(BugState.Fixed)
                .Permit(BugTrigger.Verify, BugState.Closed)
                .Permit(BugTrigger.Reopen, BugState.Reopened);

            _stateMachine.Configure(BugState.Reopened)
                .Permit(BugTrigger.AssignToDev, BugState.InProgress);

            _stateMachine.Configure(BugState.NotABug)
                .Permit(BugTrigger.Close, BugState.Closed);

            _stateMachine.Configure(BugState.Duplicate)
                .Permit(BugTrigger.Close, BugState.Closed);

            _stateMachine.Configure(BugState.CannotReproduce)
                .Permit(BugTrigger.Close, BugState.Closed);
        }

        public void Fire(BugTrigger trigger)
        {
            _stateMachine.Fire(trigger);
        }
    }

    class Program
    {
        static void Main()
        {
            var bug = new Bug();
            Console.WriteLine($"Current: {bug.State}");

            bug.Fire(BugTrigger.StartTriage);
            Console.WriteLine($"After triage: {bug.State}");

            bug.Fire(BugTrigger.AssignToDev);
            Console.WriteLine($"Assigned: {bug.State}");

            bug.Fire(BugTrigger.Fix);
            Console.WriteLine($"Fixed: {bug.State}");

            bug.Fire(BugTrigger.Verify);
            Console.WriteLine($"Closed: {bug.State}");
        }
    }
}
