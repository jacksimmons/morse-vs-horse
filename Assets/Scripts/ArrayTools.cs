/// <summary>
/// An extension class for arrays.
/// </summary>
public static class ArrayTools
{
    /// <summary>
    /// A greedy insert which moves all elements after, one place closer to the end.
    /// Deletes the final element entirely.
    /// </summary>
    public static void Insert<T>(T[] array, T toAdd, int index)
    {
        // Insert the element
        T next = array[index];
        array[index] = toAdd;

        // Move all elements (up to the second last) toward the end, by one place
        for (int i = index + 1; i < array.Length - 1; i++)
        {
            T was = array[i];
            array[i] = next;
            next = was;
        }

        // Overwrite the last element with the second last element
        array[^1] = next;
    }


    /// <summary>
    /// Gets and returns the next element of an arbitrary array in a circular array fashion.
    /// </summary>
    public static T CircularNextElement<T>(T[] arr, int current, bool right)
    {
        return arr[CircularNextIndex(current, arr.Length, right)];
    }


    /// <summary>
    /// Gets the next index of an arbitrary array in a circular array fashion.
    /// If the provided index is the last element in the array, it returns 0 (if going right).
    /// Likewise, if provided index is 0, it returns {length - 1} (if going left).
    /// </summary>
    /// <param name="current">The current index to go left/right from.</param>
    /// <param name="length">The length of the array in question.</param>
    /// <param name="right">If `true`, goes right (clockwise) around the array, otherwise goes left (anti-cw).</param>
    /// <returns>The next index.</returns>
    public static int CircularNextIndex(int current, int length, bool right)
    {
        if (right)
        {
            // Going right
            if (current + 1 >= length) return 0;
            return current + 1;
        }

        // Going left
        if (current - 1 < 0) return length - 1;
        return current - 1;
    }
}
