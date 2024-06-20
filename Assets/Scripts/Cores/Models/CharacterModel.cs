using System.Threading.Tasks;
using Cores.Models.Interfaces;
using UniRx;
using UnityEngine;
using Zenject;

namespace Cores.Models
{
    /// <summary>
    /// キャラクタに関するモデル
    /// </summary>
    public class CharacterModel : ICharacterModel
    {
        // 初期化パラメータ
        public class CharacterModelParam
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public GameObject Model { get; set; }
            public float Health { get; set; }
            public float Attack { get; set; }
            public float Speed { get; set; }

            public CharacterModelParam
            (
                int id,
                string name,
                GameObject model,
                float health,
                float attack,
                float speed
            )
            {
                Id = id;
                Name = name;
                Model = model;
                Health = health;
                Attack = attack;
                Speed = speed;
            }
        }

        public class Factory : PlaceholderFactory<CharacterModelParam, CharacterModel> { }

        public CharacterModel
        (
            CharacterModelParam characterModelParam
        )
        {
            _id = characterModelParam.Id;
            _name = characterModelParam.Name;
            _model = characterModelParam.Model;
            _health = characterModelParam.Health;
            _attack = characterModelParam.Attack;
            _speed = characterModelParam.Speed;
        }

        // 初期値
        public int Id { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public GameObject Model { get { return _model; } set { _model = value; } }
        public float Health { get { return _health; } set { _health = value; } }
        public float Attack { get { return _attack; } set { _attack = value; } }
        public float Speed { get { return _speed; } set { _speed = value; } }

        private int _id;
        private string _name;
        private GameObject _model;
        private float _health;
        private float _attack;
        private float _speed;

        // その他
        public GameObject CharacterInstance { get { return _characterInstance; } set { _characterInstance = value; } }
        public CompositeDisposable DespawnDisposables { get { return _despawnDisposables; } }

        private GameObject _characterInstance = null;
        private CompositeDisposable _despawnDisposables = new CompositeDisposable();

        // 機能
        public Subject<GameObject> OnSpawnSubject => _onSpawnSubject;
        public Subject<GameObject> OnDespawnSubject => _onDespawnSubject;

        private Subject<GameObject> _onSpawnSubject = new Subject<GameObject>();
        private Subject<GameObject> _onDespawnSubject = new Subject<GameObject>();

        public GameObject Spawn(Vector3 position, Quaternion rotation, Vector3 scale)
        {
#if UNITY_EDITOR
            Debug.Log($"{_name}を{position}に{rotation}を向いて{scale}のサイズで生成します");
#endif
            _characterInstance = GameObject.Instantiate(_model, position, rotation);
            // _characterInstance = await Spawn(id, position, rotation, scale);
            // _characterInstance.AddComponent<AudioSource>();
            OnSpawnSubject.OnNext(_characterInstance);
            return _characterInstance;
        }

        public void Despawn()
        {
#if UNITY_EDITOR
            Debug.Log($"{_name}を削除します");
#endif
            if (_characterInstance != null)
            {
                OnDespawnSubject.OnNext(_characterInstance);
                DespawnDisposables.Dispose();
                GameObject.Destroy(_characterInstance);
                _characterInstance = null;
            }
            else
            {
                Debug.LogError("モデルに対応したキャラクタが生成されていません");
            }
        }
    }
}