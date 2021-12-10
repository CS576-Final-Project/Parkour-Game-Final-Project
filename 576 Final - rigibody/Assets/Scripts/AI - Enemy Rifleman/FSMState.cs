public interface FSMState
{
    void OnEnter();

    void OnUpdate();

    void OnExit();
    
    void OnFixedUpdate();
}
