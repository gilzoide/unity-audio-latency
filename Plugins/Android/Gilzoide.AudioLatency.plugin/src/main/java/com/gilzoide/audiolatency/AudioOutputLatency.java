package com.gilzoide.audiolatency;

import android.annotation.TargetApi;
import android.media.AudioAttributes;
import android.media.AudioFormat;
import android.media.AudioManager;
import android.media.AudioTimestamp;
import android.media.AudioTrack;
import android.os.Build;
import android.util.Log;

import java.util.Arrays;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.concurrent.atomic.AtomicLong;

@TargetApi(Build.VERSION_CODES.LOLLIPOP)
public class AudioOutputLatency {
    public static boolean isSupported() {
        return Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP;
    }

    public AudioOutputLatency(int sampleRate, int bufferSize) {
        this.bufferSize = bufferSize;
        this.audioTrack = CreateAudioTrack(sampleRate, bufferSize);
    }

    private final AudioTrack audioTrack;
    private final AudioTimestamp timestamp = new AudioTimestamp();
    private final int bufferSize;
    private SilenceGenerator sampleGenerator;

    public void start() {
        stop();

        sampleGenerator = new SilenceGenerator(audioTrack, bufferSize);
        Thread writeThread = new Thread(sampleGenerator);
        writeThread.setName("AudioOutputLatency");
        writeThread.start();

        audioTrack.play();

        Log.d("AudioOutputLatency", "starting AudioTrack");
    }

    public void stop() {
        if (!isMeasuring()) {
            return;
        }

        audioTrack.stop();
        if (sampleGenerator != null) {
            sampleGenerator.stop();
            sampleGenerator = null;
        }

        Log.d("AudioOutputLatency", "stopping AudioTrack");
    }

    public boolean isMeasuring() {
        return sampleGenerator != null;
    }

    public double getOutputLatency() {
        if (sampleGenerator == null) {
            return -1;
        }

        long writtenPosition = sampleGenerator.writtenPosition.get();
        if (writtenPosition <= 0) {
            return -1;
        }

        audioTrack.getTimestamp(timestamp);

        int sampleRate = audioTrack.getSampleRate();
        long playHeadPosition = (long) (timestamp.framePosition + (System.nanoTime() - timestamp.nanoTime) * sampleRate / 1e9);

        long deltaSamples = writtenPosition - playHeadPosition;
        return (double) deltaSamples / (double) sampleRate;
    }

    private static AudioTrack CreateAudioTrack(int sampleRate, int bufferSizeInSamples) {
        return new AudioTrack(
            new AudioAttributes.Builder()
                .setUsage(AudioAttributes.USAGE_GAME)
                .setContentType(AudioAttributes.CONTENT_TYPE_UNKNOWN)
                .build(),
            new AudioFormat.Builder()
                .setEncoding(AudioFormat.ENCODING_PCM_8BIT)
                .setSampleRate(sampleRate)
                .setChannelMask(AudioFormat.CHANNEL_OUT_MONO)
                .build(),
            bufferSizeInSamples,
            AudioTrack.MODE_STREAM,
            AudioManager.AUDIO_SESSION_ID_GENERATE
        );
    }

    private static class SilenceGenerator implements Runnable {
        public AtomicLong writtenPosition = new AtomicLong(0);

        private final int bufferSize;
        private final AudioTrack audioTrack;
        private final AtomicBoolean shouldStop = new AtomicBoolean(false);

        public SilenceGenerator(AudioTrack audioTrack, int bufferSize) {
            this.audioTrack = audioTrack;
            this.bufferSize = bufferSize;
        }

        public void stop() {
            shouldStop.set(true);
        }

        @Override
        public void run() {
            byte[] buffer = new byte[bufferSize];
            Arrays.fill(buffer, (byte) 128);

            while (!shouldStop.get()) {
                int writtenBytes = audioTrack.write(buffer, 0, bufferSize);
                writtenPosition.addAndGet(writtenBytes);
            }
        }
    };
}