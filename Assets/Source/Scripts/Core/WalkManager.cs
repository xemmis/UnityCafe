using Models.Npc;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public sealed class WalkManager : MonoBehaviour
    {
        public static WalkManager Instance = null;
        private Dictionary<WalkType, List<WalkPoint>> _walkPointDict = new Dictionary<WalkType, List<WalkPoint>>();

        private void Awake()
        {
            InitializeSingleton();
            Wallet.SetMoney(10000);
        }

        public void RegisterPoint(WalkPoint walkPoint)
        {
            WalkType walkType = walkPoint.Type;
            // Если ключ еще не существует, создаем новый список
            if (!_walkPointDict.ContainsKey(walkType))
            {
                _walkPointDict[walkType] = new List<WalkPoint>();
            }

            // Добавляем точку в список
            _walkPointDict[walkType].Add(walkPoint);

            Debug.Log($"Registered walk point of type {walkType}. Total points of this type: {_walkPointDict[walkType].Count}");
        }

        public void UnregisterPoint(WalkPoint walkPoint)
        {
            WalkType walkType = walkPoint.Type;
            if (_walkPointDict.ContainsKey(walkType))
            {
                _walkPointDict[walkType].Remove(walkPoint);

                // Если список пустой, удаляем ключ
                if (_walkPointDict[walkType].Count == 0)
                {
                    _walkPointDict.Remove(walkType);
                }
            }
        }

        private void InitializeSingleton()
        {
            if (WalkManager.Instance == null)
            {
                WalkManager.Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Получить все точки определенного типа
        public List<WalkPoint> GetAllWalkPoints(WalkType walkType)
        {
            _walkPointDict.TryGetValue(walkType, out List<WalkPoint> result);
            return result ?? new List<WalkPoint>();
        }

        // Получить случайную точку определенного типа
        public WalkPoint GetRandomWalkPoint(WalkType walkType)
        {
            if (_walkPointDict.TryGetValue(walkType, out List<WalkPoint> points) && points.Count > 0)
            {
                int randomIndex = Random.Range(0, points.Count);
                return points[randomIndex];
            }

            Debug.LogWarning($"No walk points found for type: {walkType}");
            return null;
        }

        // Получить первую свободную точку определенного типа
        public WalkPoint GetFirstFreeWalkPoint(WalkType walkType)
        {
            if (_walkPointDict.TryGetValue(walkType, out List<WalkPoint> points) && points.Count > 0)
            {
                foreach (var point in points)
                {
                    if (!point.IsReserved)
                    {
                        Debug.Log($"Found free walk point of type {walkType} at {point.transform.position}");
                        return point;
                    }
                }

                Debug.LogWarning($"All walk points of type {walkType} are reserved!");
            }
            else
            {
                Debug.LogWarning($"No walk points found for type: {walkType}");
            }

            return null;
        }

        // Получить случайную свободную точку определенного типа
        public WalkPoint GetRandomFreeWalkPoint(WalkType walkType)
        {
            if (_walkPointDict.TryGetValue(walkType, out List<WalkPoint> points) && points.Count > 0)
            {
                // Собираем все свободные точки
                List<WalkPoint> freePoints = new List<WalkPoint>();
                foreach (var point in points)
                {
                    if (!point.IsReserved)
                    {
                        freePoints.Add(point);
                    }
                }

                if (freePoints.Count > 0)
                {
                    int randomIndex = Random.Range(0, freePoints.Count);
                    Debug.Log($"Found random free walk point of type {walkType} from {freePoints.Count} available");
                    return freePoints[randomIndex];
                }

                Debug.LogWarning($"All walk points of type {walkType} are reserved!");
            }
            else
            {
                Debug.LogWarning($"No walk points found for type: {walkType}");
            }

            return null;
        }

        // Получить ближайшую свободную точку от заданной позиции
        public WalkPoint GetNearestFreeWalkPoint(WalkType walkType, Vector3 position)
        {
            if (_walkPointDict.TryGetValue(walkType, out List<WalkPoint> points) && points.Count > 0)
            {
                WalkPoint nearest = null;
                float minDistance = float.MaxValue;

                foreach (var point in points)
                {
                    if (!point.IsReserved)
                    {
                        float distance = Vector3.Distance(position, point.transform.position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearest = point;
                        }
                    }
                }

                if (nearest != null)
                {
                    Debug.Log($"Found nearest free walk point at distance {minDistance}");
                }
                else
                {
                    Debug.LogWarning($"No free walk points found near position for type: {walkType}");
                }

                return nearest;
            }

            Debug.LogWarning($"No walk points found for type: {walkType}");
            return null;
        }

        // Получить количество свободных точек определенного типа
        public int GetFreeWalkPointCount(WalkType walkType)
        {
            if (_walkPointDict.TryGetValue(walkType, out List<WalkPoint> points))
            {
                int freeCount = 0;
                foreach (var point in points)
                {
                    if (!point.IsReserved)
                    {
                        freeCount++;
                    }
                }
                return freeCount;
            }

            return 0;
        }

        // Зарезервировать точку и вернуть её
        public WalkPoint ReserveWalkPoint(WalkType walkType)
        {
            WalkPoint freePoint = GetFirstFreeWalkPoint(walkType);

            if (freePoint != null)
            {
                freePoint.Reserve();
                Debug.Log($"Reserved walk point of type {walkType} at {freePoint.transform.position}");
                return freePoint;
            }

            Debug.LogWarning($"Could not reserve walk point of type {walkType} - no free points available");
            return null;
        }

        // Проверить, есть ли свободные точки определенного типа
        public bool HasFreeWalkPoints(WalkType walkType)
        {
            return GetFreeWalkPointCount(walkType) > 0;
        }

        // Получить ближайшую точку определенного типа от заданной позиции
        public WalkPoint GetNearestWalkPoint(WalkType walkType, Vector3 position)
        {
            if (_walkPointDict.TryGetValue(walkType, out List<WalkPoint> points) && points.Count > 0)
            {
                WalkPoint nearest = null;
                float minDistance = float.MaxValue;

                foreach (var point in points)
                {
                    float distance = Vector3.Distance(position, point.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = point;
                    }
                }

                return nearest;
            }

            Debug.LogWarning($"No walk points found for type: {walkType}");
            return null;
        }

        // Проверить, есть ли доступные точки определенного типа
        public bool HasWalkPoints(WalkType walkType)
        {
            return _walkPointDict.ContainsKey(walkType) && _walkPointDict[walkType].Count > 0;
        }

        // Получить количество точек определенного типа
        public int GetWalkPointCount(WalkType walkType)
        {
            return _walkPointDict.ContainsKey(walkType) ? _walkPointDict[walkType].Count : 0;
        }
    }
}