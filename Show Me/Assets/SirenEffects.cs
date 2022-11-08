using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class SirenEffects : MonoBehaviour
{
    private Volume volume;
    private DepthOfField doF;

    [Header("Vignette")]
    [SerializeField] private Vector3 maxScale = new Vector3(2.5f, 3.5f, 3.5f);
    [SerializeField] private Vector3 minScale = new Vector3(3.2f, 4.5f, 4.5f);
    [SerializeField] private float minSirenDistance = 3f;
    private float maxSirenDistance;
    private float minBlur = 2.5f;
    private float maxBlur = 1.5f;

    public Dictionary<Siren, float> sirenDistances = new Dictionary<Siren, float>();
    private Siren closestSiren;
    private UIManager manager;

    private bool active;

    private void Awake()
    {
        volume = FindObjectOfType<Volume>();
        volume.profile.TryGet<DepthOfField>(out doF);

        manager = FindObjectOfType<UIManager>();
    }

    private void Update()
    {

        closestSiren = sirenDistances.Keys.OrderBy(s => sirenDistances[s]).FirstOrDefault();
        if (closestSiren != null)
        {
            closestSiren.ApplyEffects();
            closestSiren.RotateBoatToSiren();
        }

        if (sirenDistances.Count == 0)
        {
            //manager.HideVignette();
            EnableBlur(false);
        }
        else if (sirenDistances.Count > 0)
        {
            manager.ShowVignette();
            EnableBlur(true);
        }
    }

    public void EnableBlur(bool _isActive)
    {
        doF.active = _isActive;
    }

    public void ScaleVignette(float _distance, float _maxSirenDistance)
    {
        Vector3 scale = CalculateVignetteScale(_distance, _maxSirenDistance);
        manager.ScaleVignette(scale);
    }

    public void SetBlurFromDistance(float _distance, float _maxSirenDistance)
    {
        doF.focusDistance.value = CalculateBlur(_distance, _maxSirenDistance);
    }

    private Vector3 CalculateVignetteScale(float _distance, float _maxSirenDistance)
    {
        Vector3 scale = Vector3.zero;

        float distanceRatio = (_distance - minSirenDistance) / (_maxSirenDistance - minSirenDistance);
        scale = distanceRatio * (minScale - maxScale) + maxScale;

        scale.x = Mathf.Clamp(scale.x, maxScale.x, minScale.x);
        scale.y = Mathf.Clamp(scale.y, maxScale.y, minScale.y);

        return scale;
    }

    private float CalculateBlur(float _distance, float _maxSirenDistance)
    {
        float blur = 0;

        float distanceRatio = (_distance - minSirenDistance) / (_maxSirenDistance - minSirenDistance);
        blur = distanceRatio * (minBlur - maxBlur) + maxBlur;

        blur = Mathf.Clamp(blur, maxBlur, minBlur);

        return blur;
    }

}
