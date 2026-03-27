using System;

namespace EugeneC.Utilities {

    public class HealthSystem {

        public HealthSystem(int hp) {
            MaxHealth = hp;
            Health = MaxHealth;
        }

        public int Health { get; private set; }

        public int MaxHealth { get; private set; }

        public float HealthPercentage => (float)Health / MaxHealth;

        public bool IsDead => Health <= 0;

        private event Action<int> OnHealthChanged;

        public void SubOnHealthChanged(Action<int> sub) {
            OnHealthChanged += sub;
        }

        public void UnsubOnHealthChanged(Action<int> unsub) {
            OnHealthChanged -= unsub;
        }

        private event Action<int> OnHealthMaxChanged;

        public void SubOnHealthMaxChanged(Action<int> sub) {
            OnHealthMaxChanged += sub;
        }

        public void UnsubOnHealthMaxChanged(Action<int> unsub) {
            OnHealthMaxChanged -= unsub;
        }

        private event EventHandler OnDamaged;

        public void SubOnDamaged(EventHandler sub) {
            OnDamaged += sub;
        }

        public void UnsubOnDamaged(EventHandler unsub) {
            OnDamaged -= unsub;
        }

        private event EventHandler OnHealed;

        public void SubOnHealed(EventHandler sub) {
            OnHealed += sub;
        }

        public void UnsubOnHealed(EventHandler unsub) {
            OnHealed -= unsub;
        }

        private event EventHandler OnDead;

        public void SubOnDead(EventHandler sub) {
            OnDead += sub;
        }

        public void UnsubOnDead(EventHandler unsub) {
            OnDead -= unsub;
        }

        public void Damage(int damageNumber) {
            Health -= damageNumber;
            if (Health < 0) Health = 0;

            OnHealthChanged?.Invoke(-damageNumber);
            OnDamaged?.Invoke(this, EventArgs.Empty);

            if (Health <= 0) Die();
        }

        public void Die() {
            OnDead?.Invoke(this, EventArgs.Empty);
        }

        public void Heal(int healAmount) {
            Health += healAmount;
            if (Health > MaxHealth) Health = MaxHealth;
            OnHealthChanged?.Invoke(healAmount);
            OnHealed?.Invoke(this, EventArgs.Empty);
        }

        public void OnHealthMaxChangedEvent(int hp) {
            MaxHealth += hp;
            if (MaxHealth < 0) MaxHealth = 1;
            OnHealthMaxChanged?.Invoke(hp);
        }

    }

    public interface IDamage {

        void Damaged(int tagTeam, int dmg);

    }

    public interface IHeal {

        bool CanInteract { get; }
        void Healed(int heal);

    }

}