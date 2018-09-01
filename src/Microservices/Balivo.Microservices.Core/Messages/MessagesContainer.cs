using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Balivo.Microservices.Messages
{
    public abstract class MessagesContainer
    {
        #region [ Fields ]

        private List<BrokenRuleMessage> _BrokenRules = new List<BrokenRuleMessage>();
        private List<ErrorMessage> _Errors = new List<ErrorMessage>();

        #endregion

        #region [ Properties ]

        public ReadOnlyCollection<BrokenRuleMessage> BrokenRules
        {
            get { return new ReadOnlyCollection<BrokenRuleMessage>(_BrokenRules); }
            set
            {
                if (value != null)
                {
                    foreach (var _brokenRule in value)
                    {
                        AddBrokenRule(_brokenRule);
                    }
                }
            }
        }

        public ReadOnlyCollection<ErrorMessage> Errors
        {
            get { return new ReadOnlyCollection<ErrorMessage>(_Errors); }
            set
            {
                if (value != null)
                {
                    foreach (var _error in value)
                    {
                        AddError(_error);
                    }
                }
            }
        }

        [JsonIgnore]
        public bool IsValid { get { return !(HasErrors || HasImpediments); } }

        [JsonIgnore]
        public bool HasErrors { get { return _Errors.Count() > 0; } }

        [JsonIgnore]
        public bool HasImpediments { get { return _BrokenRules.Count(lbda => lbda.Type == BrokenRuleMessageTypes.Impediment) > 0; } }

        [JsonIgnore]
        public bool HasAttentions { get { return _BrokenRules.Count(lbda => lbda.Type == BrokenRuleMessageTypes.Attention) > 0; } }

        #endregion

        public MessagesContainer()
            : base()
        {
            _BrokenRules = new List<BrokenRuleMessage>();
            _Errors = new List<ErrorMessage>();
        }

        #region [ Methods ]

        public void AddBrokenRule(BrokenRuleMessageTypes messageType, string systemKey, string message)
        {
            if (_BrokenRules.Count(lbda => lbda.Type == messageType && lbda.SystemKey.Equals(systemKey)) == 0)
                _BrokenRules.Add(new BrokenRuleMessage(messageType, systemKey, message));
        }

        public void AddBrokenRule(BrokenRuleMessage brokenRule)
        {
            if (brokenRule == null)
                throw new ArgumentNullException(nameof(brokenRule));

            AddBrokenRule(brokenRule.Type, brokenRule.SystemKey, brokenRule.Message);
        }

        public void AddBrokenRules(IEnumerable<BrokenRuleMessage> brokenRules)
        {
            if (brokenRules == null)
                throw new ArgumentNullException(nameof(brokenRules));

            foreach (var _brokenRule in brokenRules)
            {
                AddBrokenRule(_brokenRule);
            }
        }

        public BrokenRuleMessage GetBrokenRule(string systemKey)
        {
            if (string.IsNullOrWhiteSpace(systemKey))
                throw new ArgumentNullException(nameof(systemKey));

            return _BrokenRules.FirstOrDefault(lbda => lbda.SystemKey.Equals(systemKey));
        }


        public ReadOnlyCollection<BrokenRuleMessage> GetBrokenRules(BrokenRuleMessageTypes messageType)
            => new ReadOnlyCollection<BrokenRuleMessage>(_BrokenRules.Where(lbda => lbda.Type == messageType).ToList());

        public void RemoveBrokenRule(string systemKey)
        {
            if (string.IsNullOrWhiteSpace(systemKey))
                throw new ArgumentNullException(nameof(systemKey));

            _BrokenRules.RemoveAll(lbda => lbda.SystemKey.Equals(systemKey));
        }

        public void AddError(string systemKey, string message)
        {
            if (string.IsNullOrWhiteSpace(systemKey))
                throw new ArgumentNullException(nameof(systemKey));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            if (_Errors.Count(lbda => lbda.SystemKey.Equals(systemKey)) == 0)
                _Errors.Add(new ErrorMessage(systemKey, message));
        }

        public void AddError(ErrorMessage errorMessage)
        {
            if (errorMessage == null)
                throw new ArgumentNullException(nameof(errorMessage));

            AddError(errorMessage.SystemKey, errorMessage.Message);
        }

        public void AddErrors(IEnumerable<ErrorMessage> errorMessages)
        {
            if (errorMessages == null)
                throw new ArgumentNullException(nameof(errorMessages));

            foreach (var _error in errorMessages)
            {
                AddError(_error);
            }
        }

        #endregion

        public void CopyMessages(MessagesContainer messagesContainer)
        {
            if (messagesContainer != null)
            {
                AddBrokenRules(messagesContainer.BrokenRules);
                AddErrors(messagesContainer.Errors);
            }
        }
    }
}