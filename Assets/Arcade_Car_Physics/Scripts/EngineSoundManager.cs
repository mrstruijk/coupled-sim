﻿/*
 * This code is part of Arcade Car Physics for Unity by Saarg (2018)
 *
 * This is distributed under the MIT Licence (see LICENSE.md for details)
 */

using UnityEngine;


namespace VehicleBehaviour
{
    [RequireComponent(typeof(WheelVehicle))]
    [RequireComponent(typeof(AudioSource))]
    public class EngineSoundManager : MonoBehaviour
    {
        [Header("AudioClips")]
        public AudioClip starting;
        public AudioClip rolling;
        public AudioClip stopping;

        [Header("pitch parameter")]
        public float flatoutSpeed = 20.0f;
        [Range(0.0f, 3.0f)]
        public float minPitch = 0.7f;
        [Range(0.0f, 0.1f)]
        public float pitchSpeed = 0.05f;
        [Range(0.0f, 1f)]
        public float volume = 1;

        private AudioSource _source;
        private IVehicle _vehicle;


        private void Start()
        {
            _source = GetComponent<AudioSource>();
            var aiCar = GetComponent<AICar>();

            if (aiCar.isActiveAndEnabled)
            {
                _vehicle = aiCar;
            }
            else
            {
                _vehicle = GetComponent<WheelVehicle>();
            }
        }


        private void Update()
        {
            if (_vehicle.Handbrake && _source.clip == rolling)
            {
                _source.volume = volume;
                _source.clip = stopping;
                _source.loop = false;
                _source.Play();
            }

            if (!_vehicle.Handbrake && (_source.clip == stopping || _source.clip == null))
            {
                _source.volume = volume;
                _source.clip = starting;
                _source.Play();
                _source.loop = false;
                _source.pitch = 1;
            }

            if (!_vehicle.Handbrake && !_source.isPlaying)
            {
                _source.volume = volume;
                _source.clip = rolling;
                _source.loop = true;
                _source.Play();
            }

            if (_source.clip == rolling)
            {
                _source.pitch = Mathf.Lerp(_source.pitch, minPitch + Mathf.Abs(_vehicle.Speed) / flatoutSpeed, pitchSpeed);
            }
        }
    }
}