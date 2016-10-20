using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AutomatizerSQL.Bodega.Annotations;
using AutomatizerSQL.Bodega.GUI.Model;

namespace AutomatizerSQL.Bodega.GUI.Common
{
    public class FastObservableCollection : IList<NodoInventario>, INotifyCollectionChanged, INotifyPropertyChanged
    {

        public FastObservableCollection()
        {
            _listaElementos = new List<NodoInventario>();
            _addedItems = new List<NodoInventario>();
            _removedItems = new List<NodoInventario>();
        }


        private bool isBacthUpdating;
        private List<NodoInventario> _listaElementos;
        private List<NodoInventario> _addedItems;
        private List<NodoInventario> _removedItems;


        public IEnumerator<NodoInventario> GetEnumerator()
        {
            return _listaElementos.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(NodoInventario item)
        {
            _listaElementos.Add(item);
            if (!isBacthUpdating)
            {
                _addedItems.Clear();
                _addedItems.Add(item);
                OnPropertyChanged(nameof(Count));
                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _addedItems));
            }
            else
            {
                _addedItems.Add(item);
            }
        }

        public void Clear()
        {
            _listaElementos.Clear();
            _addedItems.Clear();
            OnPropertyChanged(nameof(Count));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(NodoInventario item)
        {
            return _listaElementos.Contains(item);
        }

        public void CopyTo(NodoInventario[] array, int arrayIndex)
        {
            _listaElementos.CopyTo(array, arrayIndex);
        }

        public bool Remove(NodoInventario item)
        {
            if (isBacthUpdating)
            {
                _removedItems.Add(item);
            }
            else
            {
                _removedItems.Clear();
                _removedItems.Add(item);
            }

            var indx = _listaElementos.IndexOf(item);
            var r = _listaElementos.Remove(item);

            if (r)
            {
                try
                {
                    OnPropertyChanged(nameof(Count));
                    CollectionChanged?.Invoke(this,
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, indx));
                    return true;
                }
                catch (Exception)
                {
                    throw;
                }

            }
            return false;

        }

        public int Count => _listaElementos.Count;
        public bool IsReadOnly => false;

        public int IndexOf(NodoInventario item)
        {
            return _listaElementos.IndexOf(item);
        }

        public void Insert(int index, NodoInventario item)
        {
            _listaElementos.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _listaElementos.RemoveAt(index);
        }

        public NodoInventario this[int index]
        {
            get { return _listaElementos[index]; }
            set { _listaElementos[index] = value; }
        }


        public void BeginUpdate()
        {
            isBacthUpdating = true;
        }

        public void EndUpdate()
        {
            try
            {
                isBacthUpdating = false;
                if (_addedItems.Count > 0) CollectionChanged?.Invoke(this,
                         new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _addedItems));
                _addedItems.Clear();
            }
            catch (Exception)
            {
                
                throw;
            }
            

        }


        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    
}
