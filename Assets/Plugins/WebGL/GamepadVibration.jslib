var TsumigameGamepadHaptics = {
    activeActuator: null,

    stop: function () {
        var actuator = this.activeActuator;
        this.activeActuator = null;
        if (actuator === null)
            return;

        if (typeof actuator.reset === "function") {
            var resetResult = actuator.reset();
            if (resetResult && typeof resetResult.catch === "function")
                resetResult.catch(function () {});
        } else if (typeof actuator.pulse === "function") {
            var pulseResult = actuator.pulse(0, 0);
            if (pulseResult && typeof pulseResult.catch === "function")
                pulseResult.catch(function () {});
        }
    },

    play: function (weakMagnitude, strongMagnitude, durationMilliseconds) {
        this.stop();
        if (typeof navigator.getGamepads !== "function")
            return;

        var gamepads = navigator.getGamepads();
        for (var i = 0; i < gamepads.length; i++) {
            var gamepad = gamepads[i];
            if (gamepad === null)
                continue;

            var actuator = gamepad.vibrationActuator;
            if (actuator && typeof actuator.playEffect === "function") {
                this.activeActuator = actuator;
                var effectResult = actuator.playEffect("dual-rumble", {
                    startDelay: 0,
                    duration: durationMilliseconds,
                    weakMagnitude: weakMagnitude,
                    strongMagnitude: strongMagnitude
                });
                if (effectResult && typeof effectResult.catch === "function")
                    effectResult.catch(function () {});
                return;
            }

            if (gamepad.hapticActuators && gamepad.hapticActuators.length > 0) {
                actuator = gamepad.hapticActuators[0];
                if (actuator && typeof actuator.pulse === "function") {
                    this.activeActuator = actuator;
                    var pulseResult = actuator.pulse(
                        Math.max(weakMagnitude, strongMagnitude),
                        durationMilliseconds);
                    if (pulseResult && typeof pulseResult.catch === "function")
                        pulseResult.catch(function () {});
                    return;
                }
            }
        }
    }
};

mergeInto(LibraryManager.library, {
    PlayBrowserControllerVibration: function (weakMagnitude, strongMagnitude, durationMilliseconds) {
        TsumigameGamepadHaptics.play(weakMagnitude, strongMagnitude, durationMilliseconds);
    },

    StopBrowserControllerVibration: function () {
        TsumigameGamepadHaptics.stop();
    }
});
