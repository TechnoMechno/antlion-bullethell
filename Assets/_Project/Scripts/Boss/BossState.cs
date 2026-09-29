namespace AntLion.Boss
{
    // One state of the boss's state machine. BossBrain calls Enter once when switching in, Tick every frame
    // while active, and Exit once when switching out. States tell the body (BossMovement, BossAttack) what to do;
    // they don't move or shoot themselves.
    public abstract class BossState
    {
        protected readonly BossBrain brain;

        protected BossState(BossBrain brain)
        {
            this.brain = brain;
        }

        public abstract string Name { get; }

        public virtual void Enter() { }
        public virtual void Tick() { }
        public virtual void Exit() { }
    }
}
