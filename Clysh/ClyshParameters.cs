using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Clysh
{
    public class ClyshParameters : IEnumerable<ClyshParameter>
    {
        private int lastIndexRetrieved = 0;
        private int lastIndexAdd = 0;

        public ClyshParameter[] Itens { get; private set; }

        private ClyshParameters(ClyshParameter[] itens)
        {
            Itens = itens;
        }

        public static ClyshParameters Create(params ClyshParameter[] itens)
        {
            return new ClyshParameters(itens);
        }

        public void Add(string name, int minLength, int maxLength, bool required = true)
        {
            ClyshParameter parameter = new(name, minLength, maxLength, required);

            Itens[lastIndexAdd] = parameter;

            lastIndexAdd++;
        }

        public bool WaitingForRequired()
        {
            return Itens.Any(x => x.Required && x.Data == null);
        }

        public bool WaitingForAny()
        {
            return Itens.Any(x => x.Data == null);
        }

        public ClyshParameter Get(string id)
        {
            return Itens.Single(x => x.Id == id);
        }

        public bool Has(string id)
        {
            return Itens.Any(x => x.Id == id);
        }

        public ClyshParameter Last()
        {
            ClyshParameter p = Itens[lastIndexRetrieved];
            lastIndexRetrieved++;

            return p;
        }

        public string RequiredToString()
        {
            string s = "";

            Itens.Where(x => x.Required).ToList().ForEach(k => s += k.Id + ",");

            if (s.Length > 1)
                s = s[..^1];

            return s;
        }

        public override string ToString()
        {
            string paramsText = "";

            for (int i = 0; i < Itens.Length; i++)
            {
                ClyshParameter parameter = Itens[i];
                string type = parameter.Required ? "R" : "O";
                paramsText += $"{i}:<{parameter.Id}:{type}>{(i < Itens.Length - 1 ? ", " : "")}";
            }

            if (Itens.Length > 0)
                paramsText = $"[{paramsText}]: {Itens.Length}";

            return paramsText;
        }

        public IEnumerator<ClyshParameter> GetEnumerator()
        {
            foreach (ClyshParameter parameter in Itens)
            {
                yield return parameter;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}