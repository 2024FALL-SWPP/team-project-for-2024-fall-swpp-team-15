using System.Collections.Generic;
using UnityEngine.AI;
using Yogaewonsil.Characters;

namespace Yogaewonsil.Characters {
    abstract class NPCBase: CharacterBase {
        Queue<Task> taskQueue;
        NavMeshAgent navMeshAgent;
    }

    class Task {
        // TODO: Implement Task class
    }
}