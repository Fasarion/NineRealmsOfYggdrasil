using Movement;
using Player;
using Unity.Entities;
using UnityEngine;

public partial class ChangePlayerSpeedOnAttackManagedSystem : SystemBase
{
    protected override void OnStartRunning()
    {
        EventManager.OnChangeMoveSpeed += OnChangeMoveSpeed;
        EventManager.OnResetMoveSpeed += OnResetMoveSpeed;
    }
    
    protected override void OnStopRunning()
    {
        EventManager.OnChangeMoveSpeed -= OnChangeMoveSpeed;
        EventManager.OnResetMoveSpeed -= OnResetMoveSpeed;
    }

    private void OnChangeMoveSpeed(MoveSpeedChangeData data)
    {
        bool hasBuffer = SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<PlayerMoveSpeedChangeElement> buffer);
        if (!hasBuffer) return;

        var newElement = new PlayerMoveSpeedChangeElement
        {
            CurrentTime = 0,
            MaxTime = data.Duration,
            SpeedChangeFactor = data.Factor
        };

        buffer.Add(newElement);

        // update player speed
        foreach (var moveSpeed in SystemAPI.Query<RefRW<MoveSpeedComponent>>().WithAll<PlayerTag>())
        {
            moveSpeed.ValueRW.Value *= newElement.SpeedChangeFactor;
        }
    }
    
    private void OnResetMoveSpeed()
    {
        bool hasBuffer = SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<PlayerMoveSpeedChangeElement> buffer);
        if (!hasBuffer) return;

        // revert all speed changes
        foreach (var bufferElement in buffer)
        {
            foreach (var moveSpeed in SystemAPI.Query<RefRW<MoveSpeedComponent>>().WithAll<PlayerTag>())
            {
                moveSpeed.ValueRW.Value /= bufferElement.SpeedChangeFactor;
            }
        }
        
        buffer.Clear();
    }

    protected override void OnUpdate()
    {
        bool hasBuffer = SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<PlayerMoveSpeedChangeElement> buffer);
        if (!hasBuffer) return;

        float deltaTime = SystemAPI.Time.DeltaTime;
        
        SortBuffer(ref buffer);

        for (var index = buffer.Length - 1; index >= 0; index--)
        {
            var bufferElement = buffer[index];
            bufferElement.CurrentTime += deltaTime;

            if (bufferElement.CurrentTime > bufferElement.MaxTime)
            {
                foreach (var moveSpeed in SystemAPI.Query<RefRW<MoveSpeedComponent>>().WithAll<PlayerTag>())
                {
                    moveSpeed.ValueRW.Value /= bufferElement.SpeedChangeFactor;
                }
                
                buffer.RemoveAt(index);
            }
            else
            {
                buffer[index] = bufferElement; 
            }
        }
    }
    
    private void SortBuffer(ref DynamicBuffer<PlayerMoveSpeedChangeElement> buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            for (int j = i + 1; j < buffer.Length; j++)
            {
                if (buffer.ElementAt(i).CurrentTime <= buffer.ElementAt(j).CurrentTime)
                {
                    var temp = buffer.ElementAt(j);
                    buffer.ElementAt(j) = buffer.ElementAt(i);
                    buffer.ElementAt(i) = temp;
                }
            }
        }
    }
} 