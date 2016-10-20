using System.Collections;
using System.Collections.Generic;

namespace AutomatizerSQL.Bodega
{
    public class DetallesEnumerator:IEnumerator<IDetalle>
    {
        private readonly ManejaDetalle _detalle;

        private int _current=-1;

        public DetallesEnumerator(ManejaDetalle detalle)
        {
            _detalle = detalle;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            _current++;
            return  _current < _detalle.Count;
        }

        public void Reset()
        {
            _current = -1;
        }

        public IDetalle Current => _detalle[_current];

        object IEnumerator.Current => Current;
    }
}
