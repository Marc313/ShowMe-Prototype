using System.Threading.Tasks;
using UnityEngine;

public static class AnimationManager
{
    public static async void PedalAnimation(GameObject _gameObject, float _totalSeconds)
    {
        Quaternion oldRotation = _gameObject.transform.rotation;
        Quaternion newRotation = Quaternion.Euler(_gameObject.transform.rotation.x, _gameObject.transform.rotation.y + 30, _gameObject.transform.rotation.z);
        await RotateTowards(_gameObject, newRotation, _totalSeconds * .8f);
        await RotateTowards(_gameObject, oldRotation, _totalSeconds * .2f);
    }

    private static async Task RotateTowards(GameObject _gameObject, Quaternion _newRotation, float _seconds)
    {
        Quaternion oldrotation = _gameObject.transform.rotation;
        float time = 0f;

        while (time <= _seconds)
        {
            _gameObject.transform.rotation = Quaternion.Lerp(oldrotation, _newRotation, time / _seconds);

            // Wait frame
            time += Time.deltaTime;
            await Task.Yield();
        }
    }
}
