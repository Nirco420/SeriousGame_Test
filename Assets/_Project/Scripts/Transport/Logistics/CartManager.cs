using UnityEngine;

public class CartManager : MonoBehaviour
{
    [SerializeField] private int totalCarts = 2;

    public int TotalCarts => totalCarts;
    public int availableCarts { get; private set; }

    private void Awake()
    {
        availableCarts = totalCarts;
    }

    public bool AssignCart(SegmentStorage storage)
    {
        if (availableCarts <= 0)
        {
            return false;
        }

        if (storage.AssignCart() == false)
        {
            return false;
        }

        availableCarts--;
        return true;
    }

    public bool RemoveCart(SegmentStorage storage)
    {
        if (storage.RemoveCarts() == false)
        {
            return false;
        }

        availableCarts++;
        return true;
    }

  
}
