﻿using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Diagnostics;
using Stateless;
using System.Threading.Tasks;

namespace PathwayGames.Services.Slides
{
    public enum States
    {
        Inactive, Start, ShowSlide, ShowSlideComplete, ShowRewardSlide, ShowBlankCancelableSlide, ShowBlankSlide, Paused, End
    }

    public enum Triggers
    {
        Start, NextSlide, CorrectCommision, WrongCommision, Omission, NoSlides, SlideFinished, ShowBlank, Pause, Resume
    }

    public class StateMachine : StateMachine<States, Triggers>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly TriggerWithParameters<double> _showBlankTrigger;
        private readonly TriggerWithParameters<double> _showBlankCancelableTrigger;

        public StateMachine(Func<Task> createGameAction,
            Func<Task> startGameAction,
            Func<Task> nextSlideAction,
            Func<Task> evaluateSlideResponseAction,
            Func<double, Task> blankSlideAction,
            Func<double, Task> blankSlideCancelableAction,
            Func<Task> rewardSlideAction, 
            Func<Task> endAction) : base(States.Inactive)
        {
            _showBlankTrigger = SetTriggerParameters<double>(Triggers.ShowBlank);
            _showBlankCancelableTrigger = SetTriggerParameters<double>(Triggers.Omission);

            Configure(States.Inactive)
                //.OnActivateAsync(async () => await createGameAction())
                .Permit(Triggers.Start, States.Start);

            Configure(States.Start)
                .SubstateOf(States.Inactive)
                .OnEntryAsync(async () => await startGameAction())
                .Permit(Triggers.NextSlide, States.ShowSlide);

            Configure(States.ShowSlide)
                .OnEntryAsync(async() => await nextSlideAction())
                .Permit(Triggers.SlideFinished, States.ShowSlideComplete)
                .Permit(Triggers.NoSlides, States.End)
                .Ignore(Triggers.CorrectCommision)
                .Ignore(Triggers.WrongCommision);

            Configure(States.ShowSlideComplete)
                .OnEntryAsync(async () => await evaluateSlideResponseAction())
                .Permit(Triggers.CorrectCommision, States.ShowRewardSlide)
                .Permit(Triggers.ShowBlank, States.ShowBlankSlide) //
                .Permit(Triggers.Omission, States.ShowBlankCancelableSlide);

            Configure(States.ShowRewardSlide)
                .OnEntryAsync(async () => await rewardSlideAction())
                .Permit(Triggers.ShowBlank, States.ShowBlankSlide) //
                .Ignore(Triggers.SlideFinished)
                .Ignore(Triggers.CorrectCommision);

            Configure(States.ShowBlankCancelableSlide)
                .OnEntryFrom(_showBlankCancelableTrigger, t => blankSlideCancelableAction(t))
                .Permit(Triggers.CorrectCommision, States.ShowRewardSlide)
                .Permit(Triggers.NextSlide, States.ShowSlide)
                .Ignore(Triggers.WrongCommision);

            Configure(States.ShowBlankSlide)
                .OnEntryFrom(_showBlankTrigger, t => blankSlideAction(t))
                .Permit(Triggers.SlideFinished, States.ShowSlide)
                .Ignore(Triggers.CorrectCommision);

            Configure(States.End)
                .OnEntryAsync(async() => await endAction());
            
            OnTransitioned
              (
                (t) =>
                {
                    OnPropertyChanged("State");
                }
              );

            //used to debug commands and UI components
            OnTransitioned
            (
                (t) => { Debug.WriteLine("State Machine transitioned from {0} -> {1} [{2}]", t.Source, t.Destination, t.Trigger); }
            );
        }

        public async Task ChangeStateToShowBlankSlide(double displayTime)
        {
            await FireAsync(_showBlankTrigger, displayTime);
        }

        public async Task ChangeStateToShowBlankCancelableSlide(double displayTime)
        {
            await FireAsync(_showBlankCancelableTrigger, displayTime);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}