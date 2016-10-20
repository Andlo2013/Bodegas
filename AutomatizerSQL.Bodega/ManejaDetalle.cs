using System;
using System.Collections;
using System.Collections.Generic;

namespace AutomatizerSQL.Bodega
{
    public sealed class ManejaDetalle : IEnumerable<IDetalle>, IList<IDetalle>
    {
        private const int MaxNumberOfElements = 100;
        private IDetalle[] _detalles;
        private Queue<int> _emptyArraySpacesList;
        private Hashtable _hashObjectsVsIndices;
        private Dictionary<int, int> _indicesVsPosiciones;
        private int _ultimoIndice;
        private int _capacity = MaxNumberOfElements;
        public ManejaDetalle()
        {
            ResetList();
        }
        public ManejaDetalle(int capacity)
        {
            _capacity = capacity;
            ResetList();
        }


        private void ResetList()
        {
            _indicesVsPosiciones = new Dictionary<int, int>();
            _detalles = new IDetalle[_capacity];
            _emptyArraySpacesList = new Queue<int>();
            _hashObjectsVsIndices = new Hashtable();
            UltimoIndice = -1;
        }

        public IEnumerator<IDetalle> GetEnumerator()
        {
            return new DetallesEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IDetalle item)
        {
            if (Contains(item))
            {
                throw new InvalidOperationException("Ya existe el item que intenta agregar");
            }
            else
            {
                UltimoIndice += 1;
                var i = UltimoIndice;
                if (_emptyArraySpacesList.Count > 0)
                {
                    i = _emptyArraySpacesList.Dequeue();
                }
                _indicesVsPosiciones.Add(UltimoIndice, i);
                _detalles[i] = item;
                _hashObjectsVsIndices.Add(item, UltimoIndice);
            }
        }

        public void Clear()
        {
            ResetList();
        }

        public bool Contains(IDetalle item)
        {
            return _hashObjectsVsIndices.Contains(item);
        }

        public void CopyTo(IDetalle[] array, int arrayIndex)
        {
            //throw new NotImplementedException();
            Array.Copy(_detalles, arrayIndex, array, 0, _ultimoIndice);
        }

        public bool Remove(IDetalle item)
        {
            var indx = IndexOf(item);
            if (indx == -1) return false;
            RemoveAt(indx);
            return true;
        }

        public int Count => UltimoIndice + 1;
        public bool IsReadOnly => false;

        private int UltimoIndice
        {
            get
            {
                return _ultimoIndice;
            }

            set
            {
                _ultimoIndice = value;
                if (UltimoIndice >= _capacity)
                {
                    AumentarCapacidad();        
                }
            }
        }


        private void AumentarCapacidad()
        {
            _capacity = _capacity * 2;
            var tmpMatriz =  new IDetalle[_capacity];
            Array.Copy(_detalles,tmpMatriz, _ultimoIndice );
            _detalles = tmpMatriz;
        }

        public int IndexOf(IDetalle item)
        {
            if (Contains(item))
            {
                return (int)_hashObjectsVsIndices[item];

            }
            return -1;
        }

        public void Insert(int index, IDetalle item)
        {
            if (index > UltimoIndice + 1 || index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            Add(item);
            int indiceItemCreado = UltimoIndice;
            int indiceItemMover = index;
            DeplazarYReorganizarElementos(indiceItemMover, indiceItemCreado);
        }

        private void DeplazarYReorganizarElementos(int indiceInicioDesplazamiento, int indiceItemDesplazar)
        {
            for (var i = indiceInicioDesplazamiento; i < indiceItemDesplazar; i++)
            {
                var tmpIndice = _indicesVsPosiciones[i];
                _indicesVsPosiciones[i] = _indicesVsPosiciones[indiceItemDesplazar];
                _indicesVsPosiciones[indiceItemDesplazar] = tmpIndice;
            }
        }

        public void RemoveAt(int index)
        {
            if (index > UltimoIndice || index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            for (int i = index; i < UltimoIndice; i++)
            {
                var tmpIndice = _indicesVsPosiciones[i+1];
                _indicesVsPosiciones[i+1] = _indicesVsPosiciones[i];
                _indicesVsPosiciones[i] = tmpIndice;
            }

            var posicionInvalida = _indicesVsPosiciones[UltimoIndice];
            _hashObjectsVsIndices.Remove(_detalles[ posicionInvalida ]);
            _detalles[posicionInvalida] = null;
            _emptyArraySpacesList.Enqueue(posicionInvalida);
            _indicesVsPosiciones.Remove(UltimoIndice);
            UltimoIndice --;
        }

        public IDetalle this[int index]
        {
            get { return _detalles[_indicesVsPosiciones[index]]; }
            set { _detalles[_indicesVsPosiciones[index]]= value ; }
        }

        internal void AddRange(IEnumerable<DetalleIngresoBodega> detalles)
        {
            foreach (DetalleIngresoBodega detalleIngresoBodega in detalles)
            {
                this.Add(detalleIngresoBodega);
            }
        }
    }
}
