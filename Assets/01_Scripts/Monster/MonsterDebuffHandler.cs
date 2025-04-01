using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monsters
{
    public class MonsterDebuffHandler : MonoBehaviour
    {
        private Monster monster;
        private Dictionary<DEBUFF_TYPE, Coroutine> buffCoroutines; // 현재 적용중인 디버프 코루틴
        private Dictionary<DEBUFF_TYPE, Action> finishActions;     // 디버프 종료시 실행할 액션

        public void Init(Monster monster)
        {
            this.monster = monster;
            buffCoroutines = new Dictionary<DEBUFF_TYPE, Coroutine>();
            finishActions = new Dictionary<DEBUFF_TYPE, Action>();
        }

        // 디버프 실행
        public void ExecuteDebuff(DebuffData data)
        {
            if (monster.StateMachine.IsDead) return;
            
            var type = data.DebuffType;
            
            // 이미 적용중인 같은 타입의 디버프가 있다면 종료 액션 실행 후 코루틴 중지
            if (buffCoroutines.TryGetValue(type, out var coroutine))
            {
                StopCoroutine(coroutine);
                finishActions[type]?.Invoke();
            }

            // 새로운 디버프 적용 코루틴 실행
            buffCoroutines[type] = StartCoroutine(ApplyBuff(data));
            finishActions[type] = () => EndDebuff(data);
        }

        // 디버프 효과 적용 코루틴
        private IEnumerator ApplyBuff(DebuffData data)
        {
            DebuffEffect(data);

            var elapsedTime = data.Duration;
            while (elapsedTime > 0)
            {
                elapsedTime -= Time.deltaTime;
                yield return null;
            }

            var type = data.DebuffType;
            finishActions[type]?.Invoke();
        }

        private void DebuffEffect(DebuffData data)
        {
            data.ApplyDebuff(monster);
        }

        private void EndDebuff(DebuffData data)
        {
            data.RemoveDebuff(monster);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}