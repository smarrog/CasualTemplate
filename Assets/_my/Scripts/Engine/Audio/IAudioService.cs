namespace Smr.Audio {
    public interface IAudioService {
        void Play(AudioEvent audioEvent);
        void Mute(object requester);
        void Unmute(object requester);
        AudioChannelComponent GetChannel(AudioChannelType channelType);
    }
}