mergeInto(LibraryManager.library, {
    $TsumigameGamepadHaptics: {
        activeActuator: null,
        errorReported: false,

        reportError: function (error) {
            if (this.errorReported)
                return;

            this.errorReported = true;
            if (typeof console !== "undefined" && typeof console.warn === "function")
                console.warn("Controller vibration is unavailable:", error);
        },

        stop: function () {
            var actuator = this.activeActuator;
            this.activeActuator = null;
            if (actuator === null)
                return;

            try {
                var result = null;
                if (typeof actuator.reset === "function") {
                    result = actuator.reset();
                } else if (typeof actuator.pulse === "function") {
                    result = actuator.pulse(0, 0);
                }

                if (result && typeof result.catch === "function") {
                    result.catch(function (error) {
                        TsumigameGamepadHaptics.reportError(error);
                    });
                }
            } catch (error) {
                this.reportError(error);
            }
        },

        play: function (weakMagnitude, strongMagnitude, durationMilliseconds) {
            this.stop();
            try {
                if (typeof navigator === "undefined" ||
                    typeof navigator.getGamepads !== "function")
                    return;

                var gamepads = navigator.getGamepads();
                if (!gamepads)
                    return;

                var foundGamepad = false;
                for (var i = 0; i < gamepads.length; i++) {
                    var gamepad = gamepads[i];
                    if (!gamepad)
                        continue;

                    foundGamepad = true;
                    var actuator = gamepad.vibrationActuator;
                    if (actuator && typeof actuator.playEffect === "function") {
                        this.activeActuator = actuator;
                        var effectResult = actuator.playEffect("dual-rumble", {
                            startDelay: 0,
                            duration: durationMilliseconds,
                            weakMagnitude: weakMagnitude,
                            strongMagnitude: strongMagnitude
                        });
                        if (effectResult && typeof effectResult.catch === "function") {
                            effectResult.catch(function (error) {
                                TsumigameGamepadHaptics.reportError(error);
                            });
                        }
                        return;
                    }

                    if (gamepad.hapticActuators && gamepad.hapticActuators.length > 0) {
                        actuator = gamepad.hapticActuators[0];
                        if (actuator && typeof actuator.pulse === "function") {
                            this.activeActuator = actuator;
                            var pulseResult = actuator.pulse(
                                Math.max(weakMagnitude, strongMagnitude),
                                durationMilliseconds);
                            if (pulseResult && typeof pulseResult.catch === "function") {
                                pulseResult.catch(function (error) {
                                    TsumigameGamepadHaptics.reportError(error);
                                });
                            }
                            return;
                        }
                    }
                }

                if (foundGamepad)
                    this.reportError("connected controller has no supported vibration actuator");
            } catch (error) {
                this.activeActuator = null;
                this.reportError(error);
            }
        }
    },
    PlayBrowserControllerVibration__deps: ["$TsumigameGamepadHaptics"],
    PlayBrowserControllerVibration: function (weakMagnitude, strongMagnitude, durationMilliseconds) {
        TsumigameGamepadHaptics.play(weakMagnitude, strongMagnitude, durationMilliseconds);
    },

    StopBrowserControllerVibration__deps: ["$TsumigameGamepadHaptics"],
    StopBrowserControllerVibration: function () {
        TsumigameGamepadHaptics.stop();
    }
});
