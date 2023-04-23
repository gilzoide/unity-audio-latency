package com.gilzoide.audiolatency;

import android.content.Context;
import android.media.AudioManager;
import android.os.Build;

public class AudioManagerProperties {
    public static boolean isSupported() {
        return Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR1;
    }

    public static int getOutputFramesPerBuffer(Context context) {
        if (!isSupported()) {
            return -1;
        }

        try
        {
            AudioManager audioManager = (AudioManager) context.getSystemService(Context.AUDIO_SERVICE);
            return Integer.parseInt(audioManager.getProperty(AudioManager.PROPERTY_OUTPUT_FRAMES_PER_BUFFER));
        }
        catch (NumberFormatException e) {
            return -1;
        }
    }
}