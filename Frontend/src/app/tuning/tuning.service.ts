import { Injectable } from '@angular/core';

@Injectable()
export class TuningService {

    readonly defaultTuningSettings = {
        staminaDrainModifier: {
            decimals: 1,
            label: 'Stamina Drain Modifier',
            name: 'staminaDrainModifier',
            step: 0.1,
            type: 'number',
            value: 0.5
        },
        gunSwerveThreshold: {
            decimals: 0,
            label: 'Gun Swerve Threshold',
            name: 'gunSwerveThreshold',
            step: 1,
            type: 'number',
            value: 20
        },
        updateStaminaRate: {
            decimals: 1,
            label: 'Update Stamina Rate',
            name: 'updateStaminaRate',
            step: 0.1,
            type: 'number',
            value: 0.1
        },
        rigidGravity: {
            decimals: 1,
            label: 'Rigid Gravity',
            name: 'rigidGravity',
            step: 0.1,
            type: 'number',
            value: 2.2
        },
        energyToSwayOnMove: {
            decimals: 0,
            label: 'Energy Sway On Move',
            name: 'energyToSwayOnMove',
            step: 1,
            type: 'number',
            value: 70
        },
        GuardTuning: {
            PlayerMovement: {
                speedWalk: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Walk Speed',
                    name: 'speedWalk',
                    step: 0.1,
                    type: 'number',
                    help: 'Defines a player\'s walk speed',
                    value: 2.8
                },
                speedStand: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Stand Speed',
                    name: 'speedStand',
                    step: 0.1,
                    type: 'number',
                    value: 4.0
                },
                speedCrouch: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Crouch Speed',
                    name: 'speedCrouch',
                    step: 0.1,
                    type: 'number',
                    value: 2.5
                },
                speedProne: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Prone Speed',
                    name: 'speedProne',
                    step: 0.1,
                    type: 'number',
                    value: 1
                },
                speedAim: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Aim Speed',
                    name: 'speedAim',
                    step: 0.1,
                    type: 'number',
                    value: 2.1
                },
                speedAimCrouched: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Aim Speed Crouched',
                    name: 'speedAimCrouched',
                    step: 0.1,
                    type: 'number',
                    value: 1
                },
                speedAimProne: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Aim Speed Prone',
                    name: 'speedAimProne',
                    step: 0.1,
                    type: 'number',
                    value: 1
                },
                walkAnimSpeed: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Walk Animation Speed',
                    name: 'walkAnimSpeed',
                    step: 0.1,
                    type: 'number',
                    value: 0.6
                },
                feignSpeed: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Feign Death Speed',
                    name: 'feignSpeed',
                    step: 0.1,
                    type: 'number',
                    value: 1.0
                },
                stompHeight: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Stomp Height',
                    name: 'stompHeight',
                    step: 0.1,
                    type: 'number',
                    value: 4.0
                },
                canStomp: {
                    group: 'PlayerMovement',
                    label: 'Can stomp?',
                    name: 'canStomp',
                    type: 'boolean',
                    value: true
                },
                fallHeightDeath: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Fall Height Death',
                    name: 'fallHeightDeath',
                    step: 0.1,
                    type: 'number',
                    value: 13.0
                },
                fallHeightFall: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Fall Height Fall',
                    name: 'fallHeightFall',
                    step: 0.1,
                    type: 'number',
                    value: 4.3
                },
                fallHeightCrouchMod: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Fall Height Crouch Modifier',
                    name: 'fallHeightCrouchMod',
                    step: 0.1,
                    type: 'number',
                    value: 2.3
                },
                fallHeightLand: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Fall Height Land',
                    name: 'fallHeightLand',
                    step: 0.1,
                    type: 'number',
                    value: 1.9
                },
                goombaMultiplier: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Goomba Multiplier',
                    name: 'goombaMultiplier',
                    step: 0.1,
                    type: 'number',
                    value: 3.0
                },
                jumpEnergyEffect: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Jump Energy Effect',
                    name: 'jumpEnergyEffect',
                    step: 0.1,
                    type: 'number',
                    value: -20.0
                },
                runFootStepRate: {
                    decimals: 3,
                    group: 'PlayerMovement',
                    label: 'Run Footstep Rate',
                    name: 'runFootStepRate',
                    step: 0.005,
                    type: 'number',
                    value: 0.315
                },
                walkFootStepRate: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Walk Footstep Rate',
                    name: 'walkFootStepRate',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                walkSoundMod: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Walk Sound Modifier',
                    name: 'walkSoundMod',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                walkVolumeMod: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Walk Volume Modifier',
                    name: 'walkVolumeMod',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                walkDistanceMod: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Walk Distance Modifier',
                    name: 'walkDistanceMod',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                runVolumeMod: {
                    decimals: 0,
                    group: 'PlayerMovement',
                    label: 'Run Volume Modifier',
                    name: 'runVolumeMod',
                    step: 1,
                    type: 'number',
                    value: 1
                },
                runDistanceMod: {
                    decimals: 0,
                    group: 'PlayerMovement',
                    label: 'Run Distance Modifier',
                    name: 'runDistanceMod',
                    step: 1,
                    type: 'number',
                    value: 1
                },
                allowFeignDeath: {
                    group: 'PlayerMovement',
                    label: 'Allow feign death?',
                    name: 'allowFeignDeath',
                    type: 'boolean',
                    value: true
                }
            },
            PlayerLife: {
                help: '#nolife',
                defaultHP: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Default Health Points',
                    name: 'defaultHP',
                    step: 1,
                    type: 'number',
                    value: 100
                },
                defaultEnergy: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Default Energy',
                    name: 'defaultEnergy',
                    step: 1,
                    type: 'number',
                    value: 70
                },
                defaultBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Default Balance',
                    name: 'defaultBalance',
                    step: 1,
                    type: 'number',
                    value: 70
                },
                canRespawn: {
                    group: 'PlayerLife',
                    label: 'Can respawn?',
                    name: 'canRespawn',
                    type: 'boolean',
                    value: false
                },
                defaultUnragTime: {
                    decimals: 1,
                    group: 'PlayerLife',
                    label: 'Default Unrag Time',
                    name: 'defaultUnragTime',
                    step: 0.1,
                    type: 'number',
                    value: 3.0
                },
                lowStamina: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Low Stamina Threshold',
                    name: 'lowStamina',
                    step: 1,
                    type: 'number',
                    value: 40
                },
                superLowStamina: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Super Low Stamina Threshold',
                    name: 'superLowStamina',
                    step: 1,
                    type: 'number',
                    value: 25
                },
                lowStaminaUnragMod: {
                    decimals: 2,
                    group: 'PlayerLife',
                    label: 'Low Stamina Unrag Modifier',
                    name: 'lowStaminaUnragMod',
                    step: 0.05,
                    type: 'number',
                    value: 0.85
                },
                superLowStaminaUnragMod: {
                    decimals: 1,
                    group: 'PlayerLife',
                    label: 'Super Low Stamina Unrag Modifier',
                    name: 'superLowStaminaUnragMod',
                    step: 0.1,
                    type: 'number',
                    value: 1.6
                },
                breathingThreshold: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Breathing Threshold',
                    name: 'breathingThreshold',
                    step: 1,
                    type: 'number',
                    value: 15
                },
                balanceSpeed: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Balance Speed',
                    name: 'balanceSpeed',
                    step: 1,
                    type: 'number',
                    value: 1
                },
                jumpBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Jump Balance',
                    name: 'jumpBalance',
                    step: 1,
                    type: 'number',
                    value: 5
                },
                standBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Stand Balance',
                    name: 'standBalance',
                    step: 1,
                    type: 'number',
                    value: 50
                },
                crouchBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Crouch Balance',
                    name: 'crouchBalance',
                    step: 1,
                    type: 'number',
                    value: 80
                },
                proneBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Prone Balance',
                    name: 'proneBalance',
                    step: 1,
                    type: 'number',
                    value: 100
                },
                standMoveBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Stand Move Balance',
                    name: 'standMoveBalance',
                    step: 1,
                    type: 'number',
                    value: 30
                },
                standAimMoveBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Stand Aim Move Balance',
                    name: 'standAimMoveBalance',
                    step: 1,
                    type: 'number',
                    value: 49
                },
                crouchMoveBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Crouch Move Balance',
                    name: 'crouchMoveBalance',
                    step: 1,
                    type: 'number',
                    value: 60
                },
                crouchMoveAimBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Crouch Move Aim Balance',
                    name: 'crouchMoveAimBalance',
                    step: 1,
                    type: 'number',
                    value: 70
                },
                proneMoveBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Prone Move Balance',
                    name: 'proneMoveBalance',
                    step: 1,
                    type: 'number',
                    value: 90
                },
                ziplineBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Zipline Balance',
                    name: 'ziplineBalance',
                    step: 1,
                    type: 'number',
                    value: 50
                },
                stanceEnergyEffect: {
                    decimals: 1,
                    group: 'PlayerLife',
                    label: 'Stance Energy Effect',
                    multiple: true,
                    name: 'stanceEnergyEffect',
                    step: 0.1,
                    type: 'number',
                    value: {
                        0: 3.0,
                        1: 3.0,
                        2: 3.0,
                        3: -10.0,
                        4: -25.0,
                        5: 0.0,
                        6: 0.0,
                        7: 3.0,
                        8: -25.0,
                        9: 0.0
                    }
                },
                explosionAirKillDistance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Explosion Air Kill Distance',
                    name: 'explosionAirKillDistance',
                    step: 1,
                    type: 'number',
                    value: 3
                },
                bloodDripRate: {
                    decimals: 1,
                    group: 'PlayerLife',
                    label: 'Blood Drip Rate',
                    name: 'bloodDripRate',
                    step: 0.1,
                    type: 'number',
                    value: 0.9
                },
                bloodHPThreshold: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Blood HP Threshold',
                    name: 'bloodHPThreshold',
                    step: 1,
                    type: 'number',
                    value: 30
                }
            },
            CharacterMotor: {
                movement: {
                    maxForwardSpeed: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Maximum Forwards Speed',
                        name: 'maxForwardSpeed',
                        step: 0.1,
                        type: 'number',
                        value: 4.0
                    },
                    maxSidewaysSpeed: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Maximum Sideways Speed',
                        name: 'maxSidewaysSpeed',
                        step: 0.1,
                        type: 'number',
                        value: 4.0
                    },
                    maxBackwardsSpeed: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Maximum Backwards Speed',
                        name: 'maxBackwardsSpeed',
                        step: 0.1,
                        type: 'number',
                        value: 4.0
                    },
                    maxGroundAcceleration: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Maximum Ground Acceleration',
                        name: 'maxGroundAcceleration',
                        step: 0.1,
                        type: 'number',
                        value: 20.0
                    },
                    maxAirAcceleration: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Maximum Air Acceleration',
                        name: 'maxAirAcceleration',
                        step: 0.1,
                        type: 'number',
                        value: 15.0
                    },
                    gravity: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Gravity',
                        name: 'gravity',
                        step: 0.1,
                        type: 'number',
                        value: 20.0
                    },
                    maxFallSpeed: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Maximum Fall Speed',
                        name: 'maxFallSpeed',
                        step: 0.1,
                        type: 'number',
                        value: 20.0
                    }
                },
                jumping: {
                    enabled: {
                        group: 'jumping',
                        parentGroup: 'CharacterMotor',
                        label: 'Can jump?',
                        name: 'enabled',
                        type: 'boolean',
                        value: true
                    },
                    baseHeight: {
                        decimals: 1,
                        group: 'jumping',
                        parentGroup: 'CharacterMotor',
                        label: 'Base Height',
                        help: 'How high the character jumps when pressing jump and letting go immediately',
                        name: 'baseHeight',
                        step: 0.1,
                        type: 'number',
                        value: 1.0
                    },
                    extraHeight: {
                        decimals: 1,
                        group: 'jumping',
                        parentGroup: 'CharacterMotor',
                        label: 'Extra Height',
                        help: 'ExtraHeight units (metres) are added on top when holding the jump button down longer',
                        name: 'extraHeight',
                        step: 0.1,
                        type: 'number',
                        value: 1.0
                    },
                    perpAmount: {
                        decimals: 1,
                        group: 'jumping',
                        parentGroup: 'CharacterMotor',
                        label: 'Perpendicular Amount',
                        help: 'How much the character jumps out perpendicular to the walkable surface'
                            + ' (0 = fully vertical jump, 1 = fully perpendicular)',
                        name: 'perpAmount',
                        step: 0.1,
                        type: 'number',
                        value: 0.0
                    },
                    steepPerpAmount: {
                        decimals: 1,
                        group: 'jumping',
                        parentGroup: 'CharacterMotor',
                        label: 'Steep Perpendicular Amount',
                        help: 'How much the character jumps out perpendicular to the too steep surface'
                            + ' (0 = fully vertical jump, 1 = fully perpendicular)',
                        name: 'steepPerpAmount',
                        step: 0.1,
                        type: 'number',
                        value: 0.5
                    }
                },
                sliding: {
                    enabled: {
                        group: 'sliding',
                        parentGroup: 'CharacterMotor',
                        label: 'Can slide?',
                        help: 'Whether the character slides on too steep surfaces',
                        name: 'enabled',
                        type: 'boolean',
                        value: true
                    },
                    slidingSpeed: {
                        decimals: 1,
                        group: 'sliding',
                        parentGroup: 'CharacterMotor',
                        label: 'Sliding speed',
                        help: 'How fast the character slides on steep surfaces',
                        name: 'slidingSpeed',
                        step: 0.1,
                        type: 'number',
                        value: 10.0
                    },
                    sidewaysControl: {
                        decimals: 1,
                        group: 'sliding',
                        parentGroup: 'CharacterMotor',
                        label: 'Sideways Control',
                        help: 'How much the player controls the sliding direction'
                            + ' (0.5 = the player slides sideways with half the speed of the downwards sliding speed) ',
                        name: 'sidewaysControl',
                        step: 0.1,
                        type: 'number',
                        value: 1.0
                    },
                    speedControl: {
                        decimals: 1,
                        group: 'sliding',
                        parentGroup: 'CharacterMotor',
                        label: 'Speed Control',
                        help: 'How much the player influences the sliding speed'
                            + ' (0.5 = the player speeds the sliding up to 150 % or slows it down to 50 %)',
                        name: 'speedControl',
                        step: 0.1,
                        type: 'number',
                        value: 0.4
                    }
                }
            },
            Pistol: {
                fireRate: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Fire Rate',
                    help: 'Lower values result in higher fire rates (don\'t ask)',
                    name: 'fireRate',
                    step: 0.1,
                    type: 'number',
                    value: 0.3
                },
                fireRecoverRate: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Fire Recover Rate',
                    name: 'fireRecoverRate',
                    step: 0.5,
                    type: 'number',
                    value: -1
                },
                fireDelayTime: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Fire Delay Time',
                    name: 'fireDelayTime',
                    step: 0.1,
                    type: 'number',
                    value: 0.0
                },
                maxClipAmmo: {
                    decimals: 0,
                    group: 'Pistol',
                    label: 'Maximum Rounds Per Clip',
                    name: 'maxClipAmmo',
                    step: 1,
                    type: 'number',
                    value: 15
                },
                hasSemiAuto: {
                    group: 'Pistol',
                    label: 'Has semi auto?',
                    name: 'hasSemiAuto',
                    type: 'boolean',
                    value: true
                },
                hasFullAuto: {
                    group: 'Pistol',
                    label: 'Has full auto?',
                    name: 'hasFullAuto',
                    type: 'boolean',
                    value: false
                },
                hasBurstShot: {
                    group: 'Pistol',
                    label: 'Has burst shot?',
                    name: 'hasBurstShot',
                    type: 'boolean',
                    value: false
                },
                reloadTime: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Reload Time',
                    name: 'reloadTime',
                    step: 0.1,
                    type: 'number',
                    value: 1.5
                },
                autoReload: {
                    group: 'Pistol',
                    label: 'Auto reload?',
                    name: 'autoReload',
                    type: 'boolean',
                    value: false
                },
                reloadOnEquip: {
                    group: 'Pistol',
                    label: 'Reload on equip?',
                    name: 'reloadOnEquip',
                    type: 'boolean',
                    value: false
                },
                equipTime: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Equip Time',
                    name: 'equipTime',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                shellSpeed: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Shell Speed',
                    name: 'shellSpeed',
                    step: 0.1,
                    type: 'number',
                    value: 10.0
                },
                advancedRecoil: {
                    group: 'Pistol',
                    label: 'Advanced recoil?',
                    name: 'advancedRecoil',
                    type: 'boolean',
                    value: false
                },
                hasStockRecoil: {
                    group: 'Pistol',
                    label: 'Has stock recoil?',
                    name: 'hasStockRecoil',
                    type: 'boolean',
                    value: false
                },
                stockRecoilRecoverySpeed: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Stock Recoil Recovery Speed',
                    name: 'stockRecoilRecoverySpeed',
                    step: 0.1,
                    type: 'number',
                    value: 5
                },
                recoilBalance: {
                    decimals: 0,
                    group: 'Pistol',
                    label: 'Recoil Balance',
                    name: 'recoilBalance',
                    step: 1,
                    type: 'number',
                    value: 3
                },
                precoilBalance: {
                    decimals: 0,
                    group: 'Pistol',
                    label: 'Precoil Balance',
                    name: 'precoilBalance',
                    step: 1,
                    type: 'number',
                    value: 0
                },
                focusMinimum: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Focus Minimum',
                    name: 'focusMinimum',
                    step: 1,
                    type: 'number',
                    value: 0
                },
                canFocusShot: {
                    group: 'Pistol',
                    label: 'Can focus shot?',
                    name: 'canFocusShot',
                    type: 'boolean',
                    value: false
                },
                noPrecoilZoomed: {
                    group: 'Pistol',
                    label: 'No precoil zoomed?',
                    name: 'noPrecoilZoomed',
                    type: 'boolean',
                    value: true
                },
                aimDownSightsFov: {
                    decimals: 0,
                    group: 'Pistol',
                    label: 'Aim Down Sights Field Of View',
                    name: 'aimDownSightsFov',
                    step: 1,
                    type: 'number',
                    value: 50
                },
                extraZoomingFov: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Extra Zooming Field Of View',
                    name: 'extraZoomingFov',
                    step: 1,
                    type: 'number',
                    value: 35
                },
                stanceEnergyFire: {
                    decimals: 2,
                    group: 'Pistol',
                    label: 'Stance Energy Fire',
                    multiple: true,
                    name: 'stanceEnergyFire',
                    step: 0.25,
                    type: 'number',
                    value: {
                        0: -1.0,
                        1: -1.75,
                        2: -1.5,
                        3: -1.5,
                        4: -1.0,
                        5: -1.0,
                        6: -1.0
                    }
                }
            },
            SMG: {
                fireRate: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Fire Rate',
                    help: 'Lower values result in higher fire rates (don\'t ask)',
                    name: 'fireRate',
                    step: 0.1,
                    type: 'number',
                    value: 0.3
                },
                fireRecoverRate: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Fire Recover Rate',
                    name: 'fireRecoverRate',
                    step: 0.5,
                    type: 'number',
                    value: -1
                },
                fireDelayTime: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Fire Delay Time',
                    name: 'fireDelayTime',
                    step: 0.1,
                    type: 'number',
                    value: 0.0
                },
                maxClipAmmo: {
                    decimals: 0,
                    group: 'SMG',
                    label: 'Maximum Rounds Per Clip',
                    name: 'maxClipAmmo',
                    step: 1,
                    type: 'number',
                    value: 15
                },
                hasSemiAuto: {
                    group: 'SMG',
                    label: 'Has semi auto?',
                    name: 'hasSemiAuto',
                    type: 'boolean',
                    value: true
                },
                hasFullAuto: {
                    group: 'SMG',
                    label: 'Has full auto?',
                    name: 'hasFullAuto',
                    type: 'boolean',
                    value: true
                },
                hasBurstShot: {
                    group: 'SMG',
                    label: 'Has burst shot?',
                    name: 'hasBurstShot',
                    type: 'boolean',
                    value: true
                },
                reloadTime: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Reload Time',
                    name: 'reloadTime',
                    step: 0.1,
                    type: 'number',
                    value: 1.5
                },
                autoReload: {
                    group: 'SMG',
                    label: 'Auto reload?',
                    name: 'autoReload',
                    type: 'boolean',
                    value: false
                },
                reloadOnEquip: {
                    group: 'SMG',
                    label: 'Reload on equip?',
                    name: 'reloadOnEquip',
                    type: 'boolean',
                    value: false
                },
                equipTime: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Equip Time',
                    name: 'equipTime',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                shellSpeed: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Shell Speed',
                    name: 'shellSpeed',
                    step: 0.1,
                    type: 'number',
                    value: 10.0
                },
                advancedRecoil: {
                    group: 'SMG',
                    label: 'Advanced recoil?',
                    name: 'advancedRecoil',
                    type: 'boolean',
                    value: false
                },
                hasStockRecoil: {
                    group: 'SMG',
                    label: 'Has stock recoil?',
                    name: 'hasStockRecoil',
                    type: 'boolean',
                    value: false
                },
                stockRecoilRecoverySpeed: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Stock Recoil Recovery Speed',
                    name: 'stockRecoilRecoverySpeed',
                    step: 0.1,
                    type: 'number',
                    value: 5
                },
                recoilBalance: {
                    decimals: 0,
                    group: 'SMG',
                    label: 'Recoil Balance',
                    name: 'recoilBalance',
                    step: 1,
                    type: 'number',
                    value: 3
                },
                precoilBalance: {
                    decimals: 0,
                    group: 'SMG',
                    label: 'Precoil Balance',
                    name: 'precoilBalance',
                    step: 1,
                    type: 'number',
                    value: 0
                },
                focusMinimum: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Focus Minimum',
                    name: 'focusMinimum',
                    step: 1,
                    type: 'number',
                    value: 0
                },
                canFocusShot: {
                    group: 'SMG',
                    label: 'Can focus shot?',
                    name: 'canFocusShot',
                    type: 'boolean',
                    value: false
                },
                noPrecoilZoomed: {
                    group: 'SMG',
                    label: 'No precoil zoomed?',
                    name: 'noPrecoilZoomed',
                    type: 'boolean',
                    value: true
                },
                aimDownSightsFov: {
                    decimals: 0,
                    group: 'SMG',
                    label: 'Aim Down Sights Field Of View',
                    name: 'aimDownSightsFov',
                    step: 1,
                    type: 'number',
                    value: 50
                },
                extraZoomingFov: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Extra Zooming Field Of View',
                    name: 'extraZoomingFov',
                    step: 1,
                    type: 'number',
                    value: 35
                },
                stanceEnergyFire: {
                    decimals: 2,
                    group: 'SMG',
                    label: 'Stance Energy Fire',
                    multiple: true,
                    name: 'stanceEnergyFire',
                    step: 0.25,
                    type: 'number',
                    value: {
                        0: -1.0,
                        1: -1.75,
                        2: -1.5,
                        3: -1.5,
                        4: -1.0,
                        5: -1.0,
                        6: -1.0
                    }
                }
            },
            Sniper: {
                fireRate: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Fire Rate',
                    help: 'Lower values result in higher fire rates (don\'t ask)',
                    name: 'fireRate',
                    step: 0.1,
                    type: 'number',
                    value: 0.3
                },
                fireRecoverRate: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Fire Recover Rate',
                    name: 'fireRecoverRate',
                    step: 0.5,
                    type: 'number',
                    value: -1
                },
                fireDelayTime: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Fire Delay Time',
                    name: 'fireDelayTime',
                    step: 0.1,
                    type: 'number',
                    value: 0.0
                },
                maxClipAmmo: {
                    decimals: 0,
                    group: 'Sniper',
                    label: 'Maximum Rounds Per Clip',
                    name: 'maxClipAmmo',
                    step: 1,
                    type: 'number',
                    value: 15
                },
                hasSemiAuto: {
                    group: 'Sniper',
                    label: 'Has semi auto?',
                    name: 'hasSemiAuto',
                    type: 'boolean',
                    value: true
                },
                hasFullAuto: {
                    group: 'Sniper',
                    label: 'Has full auto?',
                    name: 'hasFullAuto',
                    type: 'boolean',
                    value: false
                },
                hasBurstShot: {
                    group: 'Sniper',
                    label: 'Has burst shot?',
                    name: 'hasBurstShot',
                    type: 'boolean',
                    value: false
                },
                reloadTime: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Reload Time',
                    name: 'reloadTime',
                    step: 0.1,
                    type: 'number',
                    value: 1.5
                },
                autoReload: {
                    group: 'Sniper',
                    label: 'Auto reload?',
                    name: 'autoReload',
                    type: 'boolean',
                    value: false
                },
                reloadOnEquip: {
                    group: 'Sniper',
                    label: 'Reload on equip?',
                    name: 'reloadOnEquip',
                    type: 'boolean',
                    value: false
                },
                equipTime: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Equip Time',
                    name: 'equipTime',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                shellSpeed: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Shell Speed',
                    name: 'shellSpeed',
                    step: 0.1,
                    type: 'number',
                    value: 10.0
                },
                advancedRecoil: {
                    group: 'Sniper',
                    label: 'Advanced recoil?',
                    name: 'advancedRecoil',
                    type: 'boolean',
                    value: false
                },
                hasStockRecoil: {
                    group: 'Sniper',
                    label: 'Has stock recoil?',
                    name: 'hasStockRecoil',
                    type: 'boolean',
                    value: false
                },
                stockRecoilRecoverySpeed: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Stock Recoil Recovery Speed',
                    name: 'stockRecoilRecoverySpeed',
                    step: 0.1,
                    type: 'number',
                    value: 5
                },
                recoilBalance: {
                    decimals: 0,
                    group: 'Sniper',
                    label: 'Recoil Balance',
                    name: 'recoilBalance',
                    step: 1,
                    type: 'number',
                    value: 3
                },
                precoilBalance: {
                    decimals: 0,
                    group: 'Sniper',
                    label: 'Precoil Balance',
                    name: 'precoilBalance',
                    step: 1,
                    type: 'number',
                    value: 0
                },
                focusMinimum: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Focus Minimum',
                    name: 'focusMinimum',
                    step: 1,
                    type: 'number',
                    value: 0
                },
                canFocusShot: {
                    group: 'Sniper',
                    label: 'Can focus shot?',
                    name: 'canFocusShot',
                    type: 'boolean',
                    value: false
                },
                noPrecoilZoomed: {
                    group: 'Sniper',
                    label: 'No precoil zoomed?',
                    name: 'noPrecoilZoomed',
                    type: 'boolean',
                    value: true
                },
                aimDownSightsFov: {
                    decimals: 0,
                    group: 'Sniper',
                    label: 'Aim Down Sights Field Of View',
                    name: 'aimDownSightsFov',
                    step: 1,
                    type: 'number',
                    value: 50
                },
                extraZoomingFov: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Extra Zooming Field Of View',
                    name: 'extraZoomingFov',
                    step: 1,
                    type: 'number',
                    value: 35
                },
                stanceEnergyFire: {
                    decimals: 2,
                    group: 'Sniper',
                    label: 'Stance Energy Fire',
                    multiple: true,
                    name: 'stanceEnergyFire',
                    step: 0.25,
                    type: 'number',
                    value: {
                        0: -1.0,
                        1: -1.75,
                        2: -1.5,
                        3: -1.5,
                        4: -1.0,
                        5: -1.0,
                        6: -1.0
                    }
                }
            }
        },
        IntruderTuning: {
            PlayerMovement: {
                speedWalk: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Walk Speed',
                    name: 'speedWalk',
                    step: 0.1,
                    type: 'number',
                    help: 'Defines a player\'s walk speed',
                    value: 2.8
                },
                speedStand: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Stand Speed',
                    name: 'speedStand',
                    step: 0.1,
                    type: 'number',
                    value: 4.0
                },
                speedCrouch: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Crouch Speed',
                    name: 'speedCrouch',
                    step: 0.1,
                    type: 'number',
                    value: 2.5
                },
                speedProne: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Prone Speed',
                    name: 'speedProne',
                    step: 0.1,
                    type: 'number',
                    value: 1
                },
                speedAim: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Aim Speed',
                    name: 'speedAim',
                    step: 0.1,
                    type: 'number',
                    value: 2.1
                },
                speedAimCrouched: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Aim Speed Crouched',
                    name: 'speedAimCrouched',
                    step: 0.1,
                    type: 'number',
                    value: 1
                },
                speedAimProne: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Aim Speed Prone',
                    name: 'speedAimProne',
                    step: 0.1,
                    type: 'number',
                    value: 1
                },
                walkAnimSpeed: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Walk Animation Speed',
                    name: 'walkAnimSpeed',
                    step: 0.1,
                    type: 'number',
                    value: 0.6
                },
                feignSpeed: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Feign Death Speed',
                    name: 'feignSpeed',
                    step: 0.1,
                    type: 'number',
                    value: 1.0
                },
                stompHeight: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Stomp Height',
                    name: 'stompHeight',
                    step: 0.1,
                    type: 'number',
                    value: 4.0
                },
                canStomp: {
                    group: 'PlayerMovement',
                    label: 'Can stomp?',
                    name: 'canStomp',
                    type: 'boolean',
                    value: true
                },
                fallHeightDeath: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Fall Height Death',
                    name: 'fallHeightDeath',
                    step: 0.1,
                    type: 'number',
                    value: 13.0
                },
                fallHeightFall: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Fall Height Fall',
                    name: 'fallHeightFall',
                    step: 0.1,
                    type: 'number',
                    value: 4.3
                },
                fallHeightCrouchMod: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Fall Height Crouch Modifier',
                    name: 'fallHeightCrouchMod',
                    step: 0.1,
                    type: 'number',
                    value: 2.3
                },
                fallHeightLand: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Fall Height Land',
                    name: 'fallHeightLand',
                    step: 0.1,
                    type: 'number',
                    value: 1.9
                },
                goombaMultiplier: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Goomba Multiplier',
                    name: 'goombaMultiplier',
                    step: 0.1,
                    type: 'number',
                    value: 3.0
                },
                jumpEnergyEffect: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Jump Energy Effect',
                    name: 'jumpEnergyEffect',
                    step: 0.1,
                    type: 'number',
                    value: -20.0
                },
                runFootStepRate: {
                    decimals: 3,
                    group: 'PlayerMovement',
                    label: 'Run Footstep Rate',
                    name: 'runFootStepRate',
                    step: 0.005,
                    type: 'number',
                    value: 0.315
                },
                walkFootStepRate: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Walk Footstep Rate',
                    name: 'walkFootStepRate',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                walkSoundMod: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Walk Sound Modifier',
                    name: 'walkSoundMod',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                walkVolumeMod: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Walk Volume Modifier',
                    name: 'walkVolumeMod',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                walkDistanceMod: {
                    decimals: 1,
                    group: 'PlayerMovement',
                    label: 'Walk Distance Modifier',
                    name: 'walkDistanceMod',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                runVolumeMod: {
                    decimals: 0,
                    group: 'PlayerMovement',
                    label: 'Run Volume Modifier',
                    name: 'runVolumeMod',
                    step: 1,
                    type: 'number',
                    value: 1
                },
                runDistanceMod: {
                    decimals: 0,
                    group: 'PlayerMovement',
                    label: 'Run Distance Modifier',
                    name: 'runDistanceMod',
                    step: 1,
                    type: 'number',
                    value: 1
                },
                allowFeignDeath: {
                    group: 'PlayerMovement',
                    label: 'Allow feign death?',
                    name: 'allowFeignDeath',
                    type: 'boolean',
                    value: true
                }
            },
            PlayerLife: {
                help: '#nolife',
                defaultHP: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Default Health Points',
                    name: 'defaultHP',
                    step: 1,
                    type: 'number',
                    value: 100
                },
                defaultEnergy: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Default Energy',
                    name: 'defaultEnergy',
                    step: 1,
                    type: 'number',
                    value: 70
                },
                defaultBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Default Balance',
                    name: 'defaultBalance',
                    step: 1,
                    type: 'number',
                    value: 70
                },
                canRespawn: {
                    group: 'PlayerLife',
                    label: 'Can respawn?',
                    name: 'canRespawn',
                    type: 'boolean',
                    value: false
                },
                defaultUnragTime: {
                    decimals: 1,
                    group: 'PlayerLife',
                    label: 'Default Unrag Time',
                    name: 'defaultUnragTime',
                    step: 0.1,
                    type: 'number',
                    value: 3.0
                },
                lowStamina: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Low Stamina Threshold',
                    name: 'lowStamina',
                    step: 1,
                    type: 'number',
                    value: 40
                },
                superLowStamina: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Super Low Stamina Threshold',
                    name: 'superLowStamina',
                    step: 1,
                    type: 'number',
                    value: 25
                },
                lowStaminaUnragMod: {
                    decimals: 2,
                    group: 'PlayerLife',
                    label: 'Low Stamina Unrag Modifier',
                    name: 'lowStaminaUnragMod',
                    step: 0.05,
                    type: 'number',
                    value: 0.85
                },
                superLowStaminaUnragMod: {
                    decimals: 1,
                    group: 'PlayerLife',
                    label: 'Super Low Stamina Unrag Modifier',
                    name: 'superLowStaminaUnragMod',
                    step: 0.1,
                    type: 'number',
                    value: 1.6
                },
                breathingThreshold: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Breathing Threshold',
                    name: 'breathingThreshold',
                    step: 1,
                    type: 'number',
                    value: 15
                },
                balanceSpeed: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Balance Speed',
                    name: 'balanceSpeed',
                    step: 1,
                    type: 'number',
                    value: 1
                },
                jumpBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Jump Balance',
                    name: 'jumpBalance',
                    step: 1,
                    type: 'number',
                    value: 5
                },
                standBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Stand Balance',
                    name: 'standBalance',
                    step: 1,
                    type: 'number',
                    value: 50
                },
                crouchBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Crouch Balance',
                    name: 'crouchBalance',
                    step: 1,
                    type: 'number',
                    value: 80
                },
                proneBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Prone Balance',
                    name: 'proneBalance',
                    step: 1,
                    type: 'number',
                    value: 100
                },
                standMoveBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Stand Move Balance',
                    name: 'standMoveBalance',
                    step: 1,
                    type: 'number',
                    value: 30
                },
                standAimMoveBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Stand Aim Move Balance',
                    name: 'standAimMoveBalance',
                    step: 1,
                    type: 'number',
                    value: 49
                },
                crouchMoveBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Crouch Move Balance',
                    name: 'crouchMoveBalance',
                    step: 1,
                    type: 'number',
                    value: 60
                },
                crouchMoveAimBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Crouch Move Aim Balance',
                    name: 'crouchMoveAimBalance',
                    step: 1,
                    type: 'number',
                    value: 70
                },
                proneMoveBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Prone Move Balance',
                    name: 'proneMoveBalance',
                    step: 1,
                    type: 'number',
                    value: 90
                },
                ziplineBalance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Zipline Balance',
                    name: 'ziplineBalance',
                    step: 1,
                    type: 'number',
                    value: 50
                },
                stanceEnergyEffect: {
                    decimals: 1,
                    group: 'PlayerLife',
                    label: 'Stance Energy Effect',
                    multiple: true,
                    name: 'stanceEnergyEffect',
                    step: 0.1,
                    type: 'number',
                    value: {
                        0: 3.0,
                        1: 3.0,
                        2: 3.0,
                        3: -10.0,
                        4: -25.0,
                        5: 0.0,
                        6: 0.0,
                        7: 3.0,
                        8: -25.0,
                        9: 0.0
                    }
                },
                explosionAirKillDistance: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Explosion Air Kill Distance',
                    name: 'explosionAirKillDistance',
                    step: 1,
                    type: 'number',
                    value: 3
                },
                bloodDripRate: {
                    decimals: 1,
                    group: 'PlayerLife',
                    label: 'Blood Drip Rate',
                    name: 'bloodDripRate',
                    step: 0.1,
                    type: 'number',
                    value: 0.9
                },
                bloodHPThreshold: {
                    decimals: 0,
                    group: 'PlayerLife',
                    label: 'Blood HP Threshold',
                    name: 'bloodHPThreshold',
                    step: 1,
                    type: 'number',
                    value: 30
                }
            },
            CharacterMotor: {
                movement: {
                    maxForwardSpeed: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Maximum Forwards Speed',
                        name: 'maxForwardSpeed',
                        step: 0.1,
                        type: 'number',
                        value: 4.0
                    },
                    maxSidewaysSpeed: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Maximum Sideways Speed',
                        name: 'maxSidewaysSpeed',
                        step: 0.1,
                        type: 'number',
                        value: 4.0
                    },
                    maxBackwardsSpeed: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Maximum Backwards Speed',
                        name: 'maxBackwardsSpeed',
                        step: 0.1,
                        type: 'number',
                        value: 4.0
                    },
                    maxGroundAcceleration: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Maximum Ground Acceleration',
                        name: 'maxGroundAcceleration',
                        step: 0.1,
                        type: 'number',
                        value: 20.0
                    },
                    maxAirAcceleration: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Maximum Air Acceleration',
                        name: 'maxAirAcceleration',
                        step: 0.1,
                        type: 'number',
                        value: 15.0
                    },
                    gravity: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Gravity',
                        name: 'gravity',
                        step: 0.1,
                        type: 'number',
                        value: 20.0
                    },
                    maxFallSpeed: {
                        decimals: 1,
                        group: 'movement',
                        parentGroup: 'CharacterMotor',
                        label: 'Maximum Fall Speed',
                        name: 'maxFallSpeed',
                        step: 0.1,
                        type: 'number',
                        value: 20.0
                    }
                },
                jumping: {
                    enabled: {
                        group: 'jumping',
                        parentGroup: 'CharacterMotor',
                        label: 'Can jump?',
                        name: 'enabled',
                        type: 'boolean',
                        value: true
                    },
                    baseHeight: {
                        decimals: 1,
                        group: 'jumping',
                        parentGroup: 'CharacterMotor',
                        label: 'Base Height',
                        help: 'How high the character jumps when pressing jump and letting go immediately',
                        name: 'baseHeight',
                        step: 0.1,
                        type: 'number',
                        value: 1.0
                    },
                    extraHeight: {
                        decimals: 1,
                        group: 'jumping',
                        parentGroup: 'CharacterMotor',
                        label: 'Extra Height',
                        help: 'ExtraHeight units (metres) are added on top when holding the jump button down longer',
                        name: 'extraHeight',
                        step: 0.1,
                        type: 'number',
                        value: 1.0
                    },
                    perpAmount: {
                        decimals: 1,
                        group: 'jumping',
                        parentGroup: 'CharacterMotor',
                        label: 'Perpendicular Amount',
                        help: 'How much the character jumps out perpendicular to the walkable surface'
                            + ' (0 = fully vertical jump, 1 = fully perpendicular)',
                        name: 'perpAmount',
                        step: 0.1,
                        type: 'number',
                        value: 0.0
                    },
                    steepPerpAmount: {
                        decimals: 1,
                        group: 'jumping',
                        parentGroup: 'CharacterMotor',
                        label: 'Steep Perpendicular Amount',
                        help: 'How much the character jumps out perpendicular to the too steep surface'
                            + ' (0 = fully vertical jump, 1 = fully perpendicular)',
                        name: 'steepPerpAmount',
                        step: 0.1,
                        type: 'number',
                        value: 0.5
                    }
                },
                sliding: {
                    enabled: {
                        group: 'sliding',
                        parentGroup: 'CharacterMotor',
                        label: 'Can slide?',
                        help: 'Whether the character slides on too steep surfaces',
                        name: 'enabled',
                        type: 'boolean',
                        value: true
                    },
                    slidingSpeed: {
                        decimals: 1,
                        group: 'sliding',
                        parentGroup: 'CharacterMotor',
                        label: 'Sliding speed',
                        help: 'How fast the character slides on steep surfaces',
                        name: 'slidingSpeed',
                        step: 0.1,
                        type: 'number',
                        value: 10.0
                    },
                    sidewaysControl: {
                        decimals: 1,
                        group: 'sliding',
                        parentGroup: 'CharacterMotor',
                        label: 'Sideways Control',
                        help: 'How much the player controls the sliding direction'
                            + ' (0.5 = the player slides sideways with half the speed of the downwards sliding speed) ',
                        name: 'sidewaysControl',
                        step: 0.1,
                        type: 'number',
                        value: 1.0
                    },
                    speedControl: {
                        decimals: 1,
                        group: 'sliding',
                        parentGroup: 'CharacterMotor',
                        label: 'Speed Control',
                        help: 'How much the player influences the sliding speed'
                            + ' (0.5 = the player speeds the sliding up to 150 % or slows it down to 50 %)',
                        name: 'speedControl',
                        step: 0.1,
                        type: 'number',
                        value: 0.4
                    }
                }
            },
            Pistol: {
                fireRate: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Fire Rate',
                    help: 'Lower values result in higher fire rates (don\'t ask)',
                    name: 'fireRate',
                    step: 0.1,
                    type: 'number',
                    value: 0.3
                },
                fireRecoverRate: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Fire Recover Rate',
                    name: 'fireRecoverRate',
                    step: 0.5,
                    type: 'number',
                    value: -1
                },
                fireDelayTime: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Fire Delay Time',
                    name: 'fireDelayTime',
                    step: 0.1,
                    type: 'number',
                    value: 0.0
                },
                maxClipAmmo: {
                    decimals: 0,
                    group: 'Pistol',
                    label: 'Maximum Rounds Per Clip',
                    name: 'maxClipAmmo',
                    step: 1,
                    type: 'number',
                    value: 15
                },
                hasSemiAuto: {
                    group: 'Pistol',
                    label: 'Has semi auto?',
                    name: 'hasSemiAuto',
                    type: 'boolean',
                    value: true
                },
                hasFullAuto: {
                    group: 'Pistol',
                    label: 'Has full auto?',
                    name: 'hasFullAuto',
                    type: 'boolean',
                    value: false
                },
                hasBurstShot: {
                    group: 'Pistol',
                    label: 'Has burst shot?',
                    name: 'hasBurstShot',
                    type: 'boolean',
                    value: false
                },
                reloadTime: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Reload Time',
                    name: 'reloadTime',
                    step: 0.1,
                    type: 'number',
                    value: 1.5
                },
                autoReload: {
                    group: 'Pistol',
                    label: 'Auto reload?',
                    name: 'autoReload',
                    type: 'boolean',
                    value: false
                },
                reloadOnEquip: {
                    group: 'Pistol',
                    label: 'Reload on equip?',
                    name: 'reloadOnEquip',
                    type: 'boolean',
                    value: false
                },
                equipTime: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Equip Time',
                    name: 'equipTime',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                shellSpeed: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Shell Speed',
                    name: 'shellSpeed',
                    step: 0.1,
                    type: 'number',
                    value: 10.0
                },
                advancedRecoil: {
                    group: 'Pistol',
                    label: 'Advanced recoil?',
                    name: 'advancedRecoil',
                    type: 'boolean',
                    value: false
                },
                hasStockRecoil: {
                    group: 'Pistol',
                    label: 'Has stock recoil?',
                    name: 'hasStockRecoil',
                    type: 'boolean',
                    value: false
                },
                stockRecoilRecoverySpeed: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Stock Recoil Recovery Speed',
                    name: 'stockRecoilRecoverySpeed',
                    step: 0.1,
                    type: 'number',
                    value: 5
                },
                recoilBalance: {
                    decimals: 0,
                    group: 'Pistol',
                    label: 'Recoil Balance',
                    name: 'recoilBalance',
                    step: 1,
                    type: 'number',
                    value: 3
                },
                precoilBalance: {
                    decimals: 0,
                    group: 'Pistol',
                    label: 'Precoil Balance',
                    name: 'precoilBalance',
                    step: 1,
                    type: 'number',
                    value: 0
                },
                focusMinimum: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Focus Minimum',
                    name: 'focusMinimum',
                    step: 1,
                    type: 'number',
                    value: 0
                },
                canFocusShot: {
                    group: 'Pistol',
                    label: 'Can focus shot?',
                    name: 'canFocusShot',
                    type: 'boolean',
                    value: false
                },
                noPrecoilZoomed: {
                    group: 'Pistol',
                    label: 'No precoil zoomed?',
                    name: 'noPrecoilZoomed',
                    type: 'boolean',
                    value: true
                },
                aimDownSightsFov: {
                    decimals: 0,
                    group: 'Pistol',
                    label: 'Aim Down Sights Field Of View',
                    name: 'aimDownSightsFov',
                    step: 1,
                    type: 'number',
                    value: 50
                },
                extraZoomingFov: {
                    decimals: 1,
                    group: 'Pistol',
                    label: 'Extra Zooming Field Of View',
                    name: 'extraZoomingFov',
                    step: 1,
                    type: 'number',
                    value: 35
                },
                stanceEnergyFire: {
                    decimals: 2,
                    group: 'Pistol',
                    label: 'Stance Energy Fire',
                    multiple: true,
                    name: 'stanceEnergyFire',
                    step: 0.25,
                    type: 'number',
                    value: {
                        0: -1.0,
                        1: -1.75,
                        2: -1.5,
                        3: -1.5,
                        4: -1.0,
                        5: -1.0,
                        6: -1.0
                    }
                }
            },
            SMG: {
                fireRate: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Fire Rate',
                    help: 'Lower values result in higher fire rates (don\'t ask)',
                    name: 'fireRate',
                    step: 0.1,
                    type: 'number',
                    value: 0.3
                },
                fireRecoverRate: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Fire Recover Rate',
                    name: 'fireRecoverRate',
                    step: 0.5,
                    type: 'number',
                    value: -1
                },
                fireDelayTime: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Fire Delay Time',
                    name: 'fireDelayTime',
                    step: 0.1,
                    type: 'number',
                    value: 0.0
                },
                maxClipAmmo: {
                    decimals: 0,
                    group: 'SMG',
                    label: 'Maximum Rounds Per Clip',
                    name: 'maxClipAmmo',
                    step: 1,
                    type: 'number',
                    value: 15
                },
                hasSemiAuto: {
                    group: 'SMG',
                    label: 'Has semi auto?',
                    name: 'hasSemiAuto',
                    type: 'boolean',
                    value: true
                },
                hasFullAuto: {
                    group: 'SMG',
                    label: 'Has full auto?',
                    name: 'hasFullAuto',
                    type: 'boolean',
                    value: true
                },
                hasBurstShot: {
                    group: 'SMG',
                    label: 'Has burst shot?',
                    name: 'hasBurstShot',
                    type: 'boolean',
                    value: true
                },
                reloadTime: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Reload Time',
                    name: 'reloadTime',
                    step: 0.1,
                    type: 'number',
                    value: 1.5
                },
                autoReload: {
                    group: 'SMG',
                    label: 'Auto reload?',
                    name: 'autoReload',
                    type: 'boolean',
                    value: false
                },
                reloadOnEquip: {
                    group: 'SMG',
                    label: 'Reload on equip?',
                    name: 'reloadOnEquip',
                    type: 'boolean',
                    value: false
                },
                equipTime: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Equip Time',
                    name: 'equipTime',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                shellSpeed: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Shell Speed',
                    name: 'shellSpeed',
                    step: 0.1,
                    type: 'number',
                    value: 10.0
                },
                advancedRecoil: {
                    group: 'SMG',
                    label: 'Advanced recoil?',
                    name: 'advancedRecoil',
                    type: 'boolean',
                    value: false
                },
                hasStockRecoil: {
                    group: 'SMG',
                    label: 'Has stock recoil?',
                    name: 'hasStockRecoil',
                    type: 'boolean',
                    value: false
                },
                stockRecoilRecoverySpeed: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Stock Recoil Recovery Speed',
                    name: 'stockRecoilRecoverySpeed',
                    step: 0.1,
                    type: 'number',
                    value: 5
                },
                recoilBalance: {
                    decimals: 0,
                    group: 'SMG',
                    label: 'Recoil Balance',
                    name: 'recoilBalance',
                    step: 1,
                    type: 'number',
                    value: 3
                },
                precoilBalance: {
                    decimals: 0,
                    group: 'SMG',
                    label: 'Precoil Balance',
                    name: 'precoilBalance',
                    step: 1,
                    type: 'number',
                    value: 0
                },
                focusMinimum: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Focus Minimum',
                    name: 'focusMinimum',
                    step: 1,
                    type: 'number',
                    value: 0
                },
                canFocusShot: {
                    group: 'SMG',
                    label: 'Can focus shot?',
                    name: 'canFocusShot',
                    type: 'boolean',
                    value: false
                },
                noPrecoilZoomed: {
                    group: 'SMG',
                    label: 'No precoil zoomed?',
                    name: 'noPrecoilZoomed',
                    type: 'boolean',
                    value: true
                },
                aimDownSightsFov: {
                    decimals: 0,
                    group: 'SMG',
                    label: 'Aim Down Sights Field Of View',
                    name: 'aimDownSightsFov',
                    step: 1,
                    type: 'number',
                    value: 50
                },
                extraZoomingFov: {
                    decimals: 1,
                    group: 'SMG',
                    label: 'Extra Zooming Field Of View',
                    name: 'extraZoomingFov',
                    step: 1,
                    type: 'number',
                    value: 35
                },
                stanceEnergyFire: {
                    decimals: 2,
                    group: 'SMG',
                    label: 'Stance Energy Fire',
                    multiple: true,
                    name: 'stanceEnergyFire',
                    step: 0.25,
                    type: 'number',
                    value: {
                        0: -1.0,
                        1: -1.75,
                        2: -1.5,
                        3: -1.5,
                        4: -1.0,
                        5: -1.0,
                        6: -1.0
                    }
                }
            },
            Sniper: {
                fireRate: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Fire Rate',
                    help: 'Lower values result in higher fire rates (don\'t ask)',
                    name: 'fireRate',
                    step: 0.1,
                    type: 'number',
                    value: 0.3
                },
                fireRecoverRate: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Fire Recover Rate',
                    name: 'fireRecoverRate',
                    step: 0.5,
                    type: 'number',
                    value: -1
                },
                fireDelayTime: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Fire Delay Time',
                    name: 'fireDelayTime',
                    step: 0.1,
                    type: 'number',
                    value: 0.0
                },
                maxClipAmmo: {
                    decimals: 0,
                    group: 'Sniper',
                    label: 'Maximum Rounds Per Clip',
                    name: 'maxClipAmmo',
                    step: 1,
                    type: 'number',
                    value: 15
                },
                hasSemiAuto: {
                    group: 'Sniper',
                    label: 'Has semi auto?',
                    name: 'hasSemiAuto',
                    type: 'boolean',
                    value: true
                },
                hasFullAuto: {
                    group: 'Sniper',
                    label: 'Has full auto?',
                    name: 'hasFullAuto',
                    type: 'boolean',
                    value: false
                },
                hasBurstShot: {
                    group: 'Sniper',
                    label: 'Has burst shot?',
                    name: 'hasBurstShot',
                    type: 'boolean',
                    value: false
                },
                reloadTime: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Reload Time',
                    name: 'reloadTime',
                    step: 0.1,
                    type: 'number',
                    value: 1.5
                },
                autoReload: {
                    group: 'Sniper',
                    label: 'Auto reload?',
                    name: 'autoReload',
                    type: 'boolean',
                    value: false
                },
                reloadOnEquip: {
                    group: 'Sniper',
                    label: 'Reload on equip?',
                    name: 'reloadOnEquip',
                    type: 'boolean',
                    value: false
                },
                equipTime: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Equip Time',
                    name: 'equipTime',
                    step: 0.1,
                    type: 'number',
                    value: 0.5
                },
                shellSpeed: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Shell Speed',
                    name: 'shellSpeed',
                    step: 0.1,
                    type: 'number',
                    value: 10.0
                },
                advancedRecoil: {
                    group: 'Sniper',
                    label: 'Advanced recoil?',
                    name: 'advancedRecoil',
                    type: 'boolean',
                    value: false
                },
                hasStockRecoil: {
                    group: 'Sniper',
                    label: 'Has stock recoil?',
                    name: 'hasStockRecoil',
                    type: 'boolean',
                    value: false
                },
                stockRecoilRecoverySpeed: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Stock Recoil Recovery Speed',
                    name: 'stockRecoilRecoverySpeed',
                    step: 0.1,
                    type: 'number',
                    value: 5
                },
                recoilBalance: {
                    decimals: 0,
                    group: 'Sniper',
                    label: 'Recoil Balance',
                    name: 'recoilBalance',
                    step: 1,
                    type: 'number',
                    value: 3
                },
                precoilBalance: {
                    decimals: 0,
                    group: 'Sniper',
                    label: 'Precoil Balance',
                    name: 'precoilBalance',
                    step: 1,
                    type: 'number',
                    value: 0
                },
                focusMinimum: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Focus Minimum',
                    name: 'focusMinimum',
                    step: 1,
                    type: 'number',
                    value: 0
                },
                canFocusShot: {
                    group: 'Sniper',
                    label: 'Can focus shot?',
                    name: 'canFocusShot',
                    type: 'boolean',
                    value: false
                },
                noPrecoilZoomed: {
                    group: 'Sniper',
                    label: 'No precoil zoomed?',
                    name: 'noPrecoilZoomed',
                    type: 'boolean',
                    value: true
                },
                aimDownSightsFov: {
                    decimals: 0,
                    group: 'Sniper',
                    label: 'Aim Down Sights Field Of View',
                    name: 'aimDownSightsFov',
                    step: 1,
                    type: 'number',
                    value: 50
                },
                extraZoomingFov: {
                    decimals: 1,
                    group: 'Sniper',
                    label: 'Extra Zooming Field Of View',
                    name: 'extraZoomingFov',
                    step: 1,
                    type: 'number',
                    value: 35
                },
                stanceEnergyFire: {
                    decimals: 2,
                    group: 'Sniper',
                    label: 'Stance Energy Fire',
                    multiple: true,
                    name: 'stanceEnergyFire',
                    step: 0.25,
                    type: 'number',
                    value: {
                        0: -1.0,
                        1: -1.75,
                        2: -1.5,
                        3: -1.5,
                        4: -1.0,
                        5: -1.0,
                        6: -1.0
                    }
                }
            }
        },
        MatchMode: {
            element1Name: {
                group: 'MatchMode',
                label: 'Team A Custom Name',
                name: 'element1Name',
                type: 'text',
                value: ''
            },
            element2Name: {
                group: 'MatchMode',
                label: 'Team B Custom Name',
                name: 'element2Name',
                type: 'text',
                value: ''
            },
            roundsToWin: {
                decimals: 0,
                group: 'MatchMode',
                label: 'Rounds To Win',
                help: 'Best of 3',
                name: 'roundsToWin',
                step: 1,
                type: 'number',
                value: 2
            },
            setsToWin: {
                decimals: 0,
                group: 'MatchMode',
                label: 'Sets To Win',
                help: 'Best of 3',
                name: 'setsToWin',
                step: 1,
                type: 'number',
                value: 2
            },
            swapTeamsEverySet: {
                group: 'MatchMode',
                label: 'Swap teams every set?',
                name: 'swapTeamsEverySet',
                type: 'boolean',
                value: true
            },
            canArrestTeammates: {
                group: 'MatchMode',
                label: 'Can arrest teammates?',
                name: 'canArrestTeammates',
                type: 'boolean',
                value: false
            },
            reflectiveDamageType: {
                decimals: 0,
                group: 'MatchMode',
                label: 'Reflective Damage Type',
                help: '0 = off, 1 = round start, 2 = full round',
                name: 'reflectiveDamageType',
                step: 1,
                type: 'number',
                value: 0
            }
        }
    };

    constructor() { }

    generate(form: any, defaultForm: any = null): object {
        const generatedTuning = {};
        defaultForm = defaultForm || this.defaultTuningSettings;

        for (const key in form) {
            if (typeof form[key] === 'object' && typeof defaultForm[key].value !== 'object') {
                const temp = this.generate(form[key], defaultForm[key]);
                if (temp !== null) {
                    generatedTuning[key] = temp;
                }
            } else if (!this.equals(form[key], defaultForm[key].value)) {
                generatedTuning[key] = this.getTuningValue(form[key], defaultForm[key].decimals);
            }
        }

        return Object.keys(generatedTuning).length === 0 ? null : generatedTuning;
    }

    generateTuningForm(tuningGroup: object) {
        let form = [];

        if (tuningGroup.hasOwnProperty('name')) {
            return tuningGroup;
        } else {
            for (const key in tuningGroup) {
                if (tuningGroup[key].hasOwnProperty('name')) {
                    form.push(this.generateTuningForm(tuningGroup[key]));
                } else if (key !== 'help') {
                    form.push({ 'category': true, 'label': key.charAt(0).toUpperCase() + key.slice(1), 'help': tuningGroup[key].help });
                    form = form.concat(this.generateTuningForm(tuningGroup[key]));
                }
            }
        }

        return form;
    }

    getDefaultTuningValues(tuningGroup: object): object {
        const defaultTuningValues = {};

        for (const key in tuningGroup) {
            if (tuningGroup.hasOwnProperty(key)) {
                if (tuningGroup[key].name) {
                    defaultTuningValues[key] = tuningGroup[key].value;
                } else if (key !== 'help') {
                    defaultTuningValues[key] = this.getDefaultTuningValues(tuningGroup[key]);
                }
            }
        }

        return defaultTuningValues;
    }

    extend(deep: boolean, ...args) {
        const length = args.length;
        const extended = {};

        const merge = (obj) => {
            for (const prop in obj) {
                if (Object.prototype.hasOwnProperty.call(obj, prop)) {
                    if (deep && Object.prototype.toString.call(obj[prop]) === '[object Object]') {
                        extended[prop] = this.extend(true, extended[prop], obj[prop]);
                    } else {
                        extended[prop] = obj[prop];
                    }
                }
            }
        };

        for (let i = 0; i < length; i++) {
            merge(args[i]);
        }

        return extended;
    }

    private getTuningValue(value: any, decimals: number) {
        if (typeof value === 'boolean' || typeof value === 'string') {
            return value;
        } else if (typeof value === 'object') {
            const temp = [];

            Object.keys(value).forEach((index) => {
                if (decimals) {
                    temp.push(parseFloat(value[index]).toFixed(decimals));
                } else {
                    temp.push(Math.round(value[index]));
                }
            });

            return temp;
        } else if (decimals) {
            return parseFloat(value).toFixed(decimals);
        }

        return Math.round(value);
    }

    // Original: https://github.com/angular/angular.js/blob/6c59e770084912d2345e7f83f983092a2d305ae3/src/Angular.js#L670
    private equals(o1, o2) {
        if (o1 === o2) {
            return true;
        } else if (o1 === null || o2 === null) {
            return false;
        } else if (o1 !== o1 && o2 !== o2) {
            return true;
        }

        const t1 = typeof o1;
        const t2 = typeof o2;
        let length: number;
        let key: any;
        let keySet: object;

        // tslint:disable:triple-equals
        if (t1 == t2) {

            // tslint:disable:triple-equals
            if (t1 == 'object') {
                if (this.isArray(o1)) {
                    if (!this.isArray(o2)) {
                        return false;
                    }
                    if ((length = o1.length) == o2.length) {
                        for (key = 0; key < length; key++) {
                            if (!this.equals(o1[key], o2[key])) {
                                return false;
                            }
                        }
                        return true;
                    }
                } else {
                    keySet = {};
                    for (key in o1) {
                        if (o1.hasOwnProperty(key)) {
                            if (!this.equals(o1[key], o2[key])) {
                                return false;
                            }
                            keySet[key] = true;
                        }
                    }
                    for (key in o2) {
                        if (!keySet.hasOwnProperty(key) && o2[key] !== undefined) {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    }

    private isArray(value) {
        // tslint:disable:triple-equals
        return toString.apply(value) == '[object Array]';
    }
}
