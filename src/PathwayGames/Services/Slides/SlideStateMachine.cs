using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Diagnostics;
using Stateless;
using System.Threading.Tasks;
using PathwayGames.Infrastructure.Timer;

namespace PathwayGames.Services.Slides
{
    public enum States
    {
        Inactive, Start, ShowSlide, ShowSlideComplete, ShowRewardSlide, ShowBlankCancelableSlide, ShowBlankSlide, Paused, End, Exited
    }

    public enum Triggers
    {
        Start, NextSlide, CorrectCommision, WrongCommision, Omission, NoSlides, SlideFinished, ShowBlank, Pause, Resume, Exit
    }

    public class SlideStateMachine : StateMachine<States, Triggers>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly TriggerWithParameters<double> _showBlankTrigger;
        private readonly TriggerWithParameters<double> _showBlankCancelableTrigger;

        public SlideStateMachine(Func<Task> startGameAction,
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
                .Permit(Triggers.Exit, States.Exited) // Exit
                .Permit(Triggers.NextSlide, States.ShowSlide);

            Configure(States.ShowSlide)
                .OnEntryAsync(async() => await nextSlideAction())
                .Permit(Triggers.Exit, States.Exited) // Exit
                .Permit(Triggers.SlideFinished, States.ShowSlideComplete)
                .Permit(Triggers.NoSlides, States.End)
                .Ignore(Triggers.CorrectCommision)
                .Ignore(Triggers.WrongCommision);

            Configure(States.ShowSlideComplete)
                .OnEntryAsync(async () => await evaluateSlideResponseAction())
                .Permit(Triggers.Exit, States.Exited) // Exit
                .Permit(Triggers.CorrectCommision, States.ShowRewardSlide)
                .Permit(Triggers.ShowBlank, States.ShowBlankSlide) //
                .Permit(Triggers.Omission, States.ShowBlankCancelableSlide);

            Configure(States.ShowRewardSlide)
                .OnEntryAsync(async () => await rewardSlideAction())
                .Permit(Triggers.Exit, States.Exited) // Exit
                .Permit(Triggers.ShowBlank, States.ShowBlankSlide) //
                .Ignore(Triggers.NextSlide) ////
                .Ignore(Triggers.SlideFinished)
                .Ignore(Triggers.CorrectCommision);

            Configure(States.ShowBlankCancelableSlide)
                .OnEntryFrom(_showBlankCancelableTrigger, t => blankSlideCancelableAction(t))
                .Permit(Triggers.Exit, States.Exited) // Exit
                .Permit(Triggers.CorrectCommision, States.ShowRewardSlide)
                .Permit(Triggers.NextSlide, States.ShowSlide)
                .Ignore(Triggers.WrongCommision);

            Configure(States.ShowBlankSlide)
                .OnEntryFrom(_showBlankTrigger, t => blankSlideAction(t))
                .Permit(Triggers.Exit, States.Exited) // Exit
                .Permit(Triggers.SlideFinished, States.ShowSlide)
                .Ignore(Triggers.CorrectCommision);

            Configure(States.End)
                .OnActivateAsync(async() => await endAction())
                .Ignore(Triggers.CorrectCommision);

            Configure(States.Exited)
                .Ignore(Triggers.ShowBlank)
                .Ignore(Triggers.NextSlide)
                .Ignore(Triggers.SlideFinished);

            OnTransitioned
            (
                (t) => { OnPropertyChanged("State"); }
            );

            //used to debug commands and UI components
            OnTransitioned
            (
                (t) => { Debug.WriteLine("{0:HH:mm:ss.fff} - State Machine transitioned from {1} -> {2} [{3}]", 
                    TimerClock.Now, t.Source, t.Destination, t.Trigger); }
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
