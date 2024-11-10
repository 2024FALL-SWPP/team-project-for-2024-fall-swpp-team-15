using namespace Yogaewonsil.Characters;
{
    abstract class NPCBase extends CharacterBase {
        taskQueue: Queue<Task>;
        navMeshAgent: NavMeshAgent;
    }
}