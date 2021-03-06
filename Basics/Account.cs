﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Basics
{
    public class Account
    {
        private float amount = 0;
        public float Amount => amount;
        public float Max;

        public Account(float _initialDeposit=0, float _max=float.MaxValue)
        {
            Max = _max;
            if(_initialDeposit > 0)
                Deposit(_initialDeposit);
        }

        /// <summary>
        /// Deposits as much of the given amount into the account as possible given the maximum size of the account
        /// </summary>
        /// <param name="_amount">The amount to attempt to deposit</param>
        /// <returns>The amount that was not deposited</returns>
        public float Deposit(float _amount)
        {
            if (_amount <= 0)
                return 0;
            if(amount + _amount > Max)
            {
                var diff = amount + _amount - Max;
                amount = Max;
                return diff;
            }
            else
            {
                amount += _amount;
                return 0;
            }
        }

        /// <summary>
        /// Withdraws as much from the account as possible until it is empty.
        /// </summary>
        /// <param name="_amount">The amount to attempt to withdraw</param>
        /// <returns>The amount that was successfully withdrawn</returns>
        public float Withdraw(float _amount)
        {
            if (_amount <= 0)
                return 0;
            _amount = Basics.Utils.Min(amount, _amount);
            amount -= _amount;
            return _amount;
        }
        public float Withdraw() => Withdraw(amount);
        public void Empty() => Withdraw();
        public bool IsEmpty => amount <= 0;
        public float Remaining => Max - amount;

        /// <summary>
        /// Transfers as much of the given amount as possible from this account to another
        /// </summary>
        /// <param name="_amount">The amount that is being transferred</param>
        /// <param name="_recipient">The account that will receive the funds</param>
        /// <returns>The amount successfully withdrawn & deposited</returns>
        public float Transfer(float _amount, Account _recipient)
        {
            _amount = Withdraw(_amount);
            var undepositedAmount = _recipient.Deposit(_amount);
            if (undepositedAmount > 0)
            {
                var depositedBackAmount = Deposit(undepositedAmount);
                if (depositedBackAmount > 0)
                    throw new Exception($"If we can't deposit an amount that is less than the amount we just withdrew, then Withdraw or Deposit is changing the Max value which isn't allowed. Amount successfully deposited = {depositedBackAmount}");
            }
            return _amount - undepositedAmount;
        }

        public float TransferAll(Account _recipient) => Transfer(amount, _recipient);
    }
}
