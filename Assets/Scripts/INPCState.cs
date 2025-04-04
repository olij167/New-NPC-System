public interface INPCState
{
    string StateName { get; }
    void Enter(NPC npc);
    void UpdateState(NPC npc);
    void Exit(NPC npc);
}