# V-Stage Quest3 Public 🎭

**차세대 AI 기반 실시간 버츄얼 콘서트 플랫폼 - Meta Quest3 관객용**

![Unity](https://img.shields.io/badge/Unity-6000.1.2f1-000000?style=for-the-badge&logo=unity)
[![Meta Quest 3](https://img.shields.io/badge/Meta_Quest_3-Compatible-blue.svg)](https://www.meta.com/quest/quest-3/)
[![Photon Fusion](https://img.shields.io/badge/Photon_Fusion-Networking-green.svg)](https://www.photonengine.com/fusion)
[![XR Hands](https://img.shields.io/badge/Unity_XR_Hands-Tracking-orange.svg)](https://docs.unity3d.com/Packages/com.unity.xr.hands@1.5/manual/index.html)

## 📖 개요

V-Stage Quest3는 메타버스 환경에서 실시간 VR 콘서트를 경험할 수 있는 혁신적인 플랫폼입니다. AI 기반 실시간 감정 분석과 멀티플레이어 네트워킹을 통해 관객들이 능동적으로 참여할 수 있는 몰입형 버츄얼 콘서트 경험을 제공합니다.

> **V-Stage 생태계**: 이 프로젝트는 **관객용 클라이언트** 애플리케이션으로, Meta Quest 3 사용자들이 VR 콘서트에 참여할 때 사용합니다. 공연자는 별도의 **Host 애플리케이션** ([V-Stage Win Public](https://github.com/carlton368/vstage_win_public))을 통해 HTC Vive 트래커와 Windows 환경에서 실시간 공연을 제어합니다.

### ✨ 주요 특징

- **🎤 AI 기반 실시간 관객 반응 분석**: 음성과 제스처를 통한 감정 인식
- **🌐 멀티플레이어 VR 환경**: Photon Fusion 기반 실시간 동기화
- **🤲 직관적인 VR 인터랙션**: Unity XR Hands를 활용한 자연스러운 제스처 제어
- **🎵 실시간 오디오 처리**: WebSocket 기반 AI 서버 연동
- **💡 인터랙티브 라이트스틱**: 물리 기반 응원 도구
- **👤 아바타 시스템**: VRIK 기반 실시간 아바타 동기화

## 🎯 대상 사용자

- **관객 (Meta Quest3 사용자)**: VR 환경에서 콘서트 관람 및 인터랙션
- **퍼포머 (호스트)**: VR 트래커를 통한 실시간 공연 제어
- **개발자**: VR 콘서트 플랫폼 확장 및 커스터마이징

## 🛠️ 시스템 요구사항

### Client (관객용 - Meta Quest 3)
#### 하드웨어
- **VR 헤드셋**: Meta Quest 3 (필수)
- **핸드 트래킹**: Meta Quest 3 내장 핸드 트래킹 활성화
- **네트워크**: 안정적인 Wi-Fi 연결 (5GHz 권장)
- **저장공간**: 최소 2GB 여유 공간

#### 소프트웨어
- **Quest OS**: v57+ (Meta Quest 3 최신 펌웨어)
- **개발자 모드**: APK 설치 시 필요

### Host (공연자용 - 별도 프로젝트)
#### 하드웨어
- **PC**: Windows 10/11 (64-bit)
- **VR 트래커**: HTC Vive Tracker 3.0 (3~7개)
- **베이스 스테이션**: SteamVR 트래킹용
- **CPU**: Intel i7-9700K 또는 AMD Ryzen 7 3700X 이상
- **RAM**: 32GB 이상 권장 (VR 트래킹 처리용)
- **GPU**: RTX 3070 이상 권장 (실시간 렌더링용)


### 개발 환경 (Unity Editor)
#### 하드웨어
- **PC**: Windows 10/11 또는 macOS
- **RAM**: 16GB 이상 권장
- **GPU**: GTX 1660 이상 권장
- **저장공간**: 10GB 이상 여유 공간

#### 소프트웨어
- **Unity**: 6000.1.2f1 이상 (Quest 3 빌드용)
- **Unity XR Plugin Management**: 4.4.1+
- **Meta XR SDK**: 68.0.0+
- **Android Build Support**: Unity 모듈

## 📦 패키지 의존성

### 핵심 패키지
```json
{
  "com.unity.xr.interaction.toolkit": "3.1.2",
  "com.unity.xr.hands": "1.5.1",
  "com.unity.xr.meta-openxr": "2.2.0",
  "com.unity.inputsystem": "1.14.0",
  "com.unity.cinemachine": "3.1.4",
  "com.unity.render-pipelines.universal": "17.1.0"
}
```

### Third-party 패키지
- **Photon Fusion**: 네트워킹 프레임워크
- **MagicaCloth2**: 의상 물리 시뮬레이션
- **Final IK**: VR 아바타 IK 솔루션
- **lilToon**: NPR 렌더링 셰이더
- **NativeWebSocket**: AI 서버 통신

## 🏗️ 아키텍처

### V-Stage 전체 시스템 구조
```
┌─────────────────────────────────────────────────────────────────┐
│                        V-Stage 생태계                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  🎭 Host Application (Windows + VR Trackers)                   │
│  ├── Unity 6000.1.2f1                                          │
│  ├── HTC Vive / SteamVR 트래커                                  │
│  ├── Final IK (100+ 본 동기화)                                  │
│  ├── 페이셜 트래킹 (6채널)                                        │
│  └── AI 반응 시스템                                             │
│                     ↓                                           │
│              [Photon Fusion]                                    │
│                     ↓                                           │
│  📱 Client Application (Meta Quest 3) - 이 프로젝트              │
│  ├── Unity 2023.3.0f1                                          │
│  ├── Unity XR Hands 트래킹                                      │
│  ├── VR 인터랙션 (라이트스틱, 제스처)                             │
│  ├── 실시간 아바타 동기화                                        │
│  └── AI 기반 관객 반응 분석                                      │
│                     ↓                                           │
│              [WebSocket]                                        │
│                     ↓                                           │
│  🤖 AI 분석 서버                                                │
│  ├── 음성 감정 분석                                              │
│  ├── 제스처 인식                                                │
│  ├── 키워드 추출                                                │
│  └── 실시간 피드백 생성                                          │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### Host vs Client 비교

| 구분 | Host (공연자용) | Client (관객용) |
|------|----------------|----------------|
| **플랫폼** | Windows + VR Trackers | Meta Quest 3 |
| **Unity 버전** | Unity 6000.1.2f1 | Unity 2023.3.0f1 |
| **주요 역할** | VR 공연 제어 및 송출 | VR 콘서트 관람 및 참여 |
| **트래킹** | HTC Vive/SteamVR (100+본) | Unity XR Hands |
| **페이셜 트래킹** | 6채널 실시간 캡처 | 네트워크 동기화 수신 |
| **AI 기능** | 관객 반응 수집 및 처리 | 개인 반응 송신 |
| **네트워킹** | Photon Fusion 호스트 | Photon Fusion 클라이언트 |

### 주요 컴포넌트

#### 1. **네트워킹 시스템**
- `BasicSpawner.cs`: Photon Fusion 기반 멀티플레이어 관리
- `VRIKNetworkPlayer.cs`: VR 아바타 실시간 동기화
- `TimelineController.cs`: 네트워크 동기화된 타이밍 제어

#### 2. **VR 인터랙션**
- `HandGesture.cs`: Unity XR Hands 기반 제스처 인식
- `LightStickComponent.cs`: 물리 기반 라이트스틱 조작
- `RecordingManager.cs`: 제스처 기반 음성 녹음

#### 3. **AI 연동**
- `WebSocketVoiceClient.cs`: 실시간 AI 서버 통신
- `AIResponseStore.cs`: AI 분석 결과 중앙 관리
- `EmotionColorMapper.cs`: 감정-색상 매핑 시스템

#### 4. **페이셜 트래킹**
- `SimpleShinanoFacialTracking.cs`: 네트워크 동기화된 표정 제어
- 6가지 핵심 블렌드셰이프 지원 (미소, 슬픔, O모양 등)

## 🚀 설치 및 실행

### 📋 사전 준비사항

#### Host 애플리케이션 설정 (공연자)
1. **Host 프로젝트 설치**: [V-Stage Win Public](https://github.com/carlton368/vstage_win_public) 리포지토리에서 Host 애플리케이션을 먼저 설정
2. **VR 트래커 준비**: HTC Vive Tracker 3.0과 베이스 스테이션 설정
3. **SteamVR 구성**: 트래커 페어링 및 캘리브레이션 완료

### 🎯 Client 애플리케이션 설정 (관객용)

#### 1. 개발 환경 설정
```bash
# 1) Unity Hub 설치
# 2) Unity 2023.3.0f1 이상 버전 설치
# 3) Android Build Support 모듈 포함 설치
```

#### 2. Meta Quest 3 개발자 설정
```bash
# 1) Meta Quest Developer Hub 설치
#    https://developer.oculus.com/downloads/package/oculus-developer-hub-win/

# 2) Meta 개발자 계정 생성 및 조직 생성
#    https://developer.oculus.com/

# 3) Quest 3를 개발자 모드로 설정
#    스마트폰 Meta Quest 앱 → 헤드셋 설정 → 개발자 모드 활성화

# 4) USB 디버깅 활성화
#    Quest 3에서 USB 연결 시 디버깅 허용
```

#### 3. 프로젝트 설정
```bash
# 1) 프로젝트 클론
git clone https://github.com/your-repo/vstage_quest3_public.git
cd vstage_quest3_public

# 2) Unity에서 프로젝트 열기
# Unity Hub → Open → 프로젝트 폴더 선택

# 3) 패키지 자동 설치 확인
# Package Manager에서 필수 패키지들이 모두 설치되었는지 확인
```

#### 4. Quest 3 빌드 설정
```bash
# Unity에서 다음 설정:
# 1) File → Build Settings
# 2) Platform: Android 선택 후 Switch Platform
# 3) Player Settings 설정:
#    - Company Name: 본인 회사명
#    - Product Name: V-Stage Quest3
#    - Minimum API Level: Android 10.0 (API 29)
#    - Target API Level: Automatic (Highest Installed)
# 4) XR Plug-in Management:
#    - Android 탭에서 Oculus 체크
#    - Initialize XR on Startup 체크
```

#### 5. 실행 방법

##### 🎮 Unity Editor에서 테스트
```bash
# 1) Unity Editor에서 App_Final.unity 씬 열기
# 2) Play 버튼 클릭 (VR 시뮬레이션)
# 참고: Editor에서는 핸드 트래킹이 정확하지 않을 수 있음
```

##### 📱 Quest 3에 직접 설치
```bash
# 1) Quest 3를 USB-C로 PC에 연결
# 2) Unity Build Settings → Build And Run
# 3) APK 빌드 후 자동으로 Quest 3에 설치 및 실행

# 또는 APK만 생성 후 수동 설치:
# Build → APK 파일 생성 → Developer Hub에서 설치
```

### 🌐 전체 시스템 실행 순서

1. **Host 준비**: Host PC에서 V-Stage Win 애플리케이션 실행
2. **네트워크 방 생성**: Host에서 Photon Fusion 방 생성
3. **Client 연결**: Quest 3에서 앱 실행 후 방 참가
4. **AI 서버 연결**: WebSocket을 통한 AI 분석 서버 연결 확인
5. **공연 시작**: Host에서 타임라인 재생으로 공연 시작

## 📁 프로젝트 구조

```
vstage_quest3_public/
├── Assets/
│   ├── 01_Scenes/              # Unity 씬 파일들
│   │   ├── App_Final.unity     # 메인 콘서트 씬
│   │   ├── Network.unity       # 네트워킹 테스트 씬
│   │   └── ...
│   ├── 02_Prefabs/             # 게임 오브젝트 프리팹들
│   │   ├── XR/                 # VR 관련 프리팹
│   │   ├── Audio/              # 오디오 관련 프리팹
│   │   └── Interactables/      # 인터랙션 오브젝트
│   ├── 03_Scripts/             # C# 스크립트들
│   │   ├── Api/                # AI 서버 통신
│   │   ├── Photon/             # 네트워킹 로직
│   │   ├── Gestures/           # 제스처 인식
│   │   ├── UI/                 # 사용자 인터페이스
│   │   └── ...
│   ├── Art/                    # 3D 모델, 텍스처, 애니메이션
│   ├── Audio/                  # 사운드 파일들
│   └── ThirdParty/             # 서드파티 에셋들
├── Packages/
│   └── manifest.json           # 패키지 의존성 정의
├── ProjectSettings/            # Unity 프로젝트 설정
└── README.md                   # 이 파일
```

## 🎮 사용법

### 관객 모드 (Quest 3)
1. **헤드셋 착용** 후 앱 실행
2. **핸드 트래킹 활성화** (설정에서 확인)
3. **로비에서 대기** 후 호스트 연결
4. **제스처로 상호작용**:
   - ✋ 손을 올리고 말하기 → 음성 녹음
   - 👏 응원봉 흔들기 제스처 → 응원봉 발광 효과
   - 🤏 라이트스틱 잡기 → 응원봉 조작

### 호스트 모드 (VR 트래커 필요)
1. **VR 트래커 설정** 및 캘리브레이션
2. **네트워크 방 생성**
3. **타임라인 재생**으로 공연 시작
4. **실시간 아바타 제어**

## 🔧 개발자 가이드

### 🏗️ 아키텍처 패턴

V-Stage Quest3는 다음 설계 패턴을 사용합니다:

- **MVC 패턴**: UI와 로직 분리
- **Observer 패턴**: AI 반응 시스템의 이벤트 기반 처리
- **Singleton 패턴**: 중앙 데이터 관리 (AIResponseStore)
- **Component 시스템**: Unity의 모듈화된 컴포넌트 구조

### 📚 주요 API 레퍼런스

#### 🤲 제스처 인식 시스템
```csharp
// 커스텀 제스처 핸들러 생성
public class CustomGestureHandler : MonoBehaviour
{
    [SerializeField] private StaticHandGesture gestureToDetect;
    [SerializeField] private UnityEvent OnGestureDetected;

    void OnEnable()
    {
        gestureToDetect.GesturePerformed.AddListener(OnGestureDetected);
    }

    void OnDisable()
    {
        gestureToDetect.GesturePerformed.RemoveListener(OnGestureDetected);
    }

    private void OnGestureDetected(StaticHandGesture gesture)
    {
        Debug.Log($"제스처 감지: {gesture.name}");
        OnGestureDetected?.Invoke();

        // 햅틱 피드백 추가
        StartCoroutine(TriggerHapticFeedback());
    }

    private IEnumerator TriggerHapticFeedback()
    {
        // Quest 3 컨트롤러 진동 (핸드 트래킹 시에는 작동하지 않음)
        OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.Touch);
        yield return new WaitForSeconds(0.1f);
        OVRInput.SetControllerVibration(0f, 0f, OVRInput.Controller.Touch);
    }
}
```

#### 🤖 AI 분석 결과 처리
```csharp
public class AdvancedEmotionReactor : MonoBehaviour
{
    [SerializeField] private ParticleSystem emotionParticles;
    [SerializeField] private AudioSource emotionAudio;
    [SerializeField] private Light environmentLight;

    void Start()
    {
        // AI 분석 결과 구독
        AIResponseStore.Instance.OnKeywordDataUpdated.AddListener(HandleKeywords);
        AIResponseStore.Instance.OnEmotionDataUpdated.AddListener(HandleEmotion);
        AIResponseStore.Instance.OnVoiceRecognitionResult.AddListener(HandleVoiceResult);
    }

    private void HandleEmotion(EmotionData emotion)
    {
        // 감정 색상 매핑
        Color emotionColor = EmotionColorMapper.GetColor(emotion.dominantEmotion);

        // 환경 조명 변경
        if (environmentLight != null)
        {
            StartCoroutine(ChangeEnvironmentColor(emotionColor));
        }

        // 파티클 효과
        if (emotionParticles != null)
        {
            var main = emotionParticles.main;
            main.startColor = emotionColor;
            emotionParticles.Play();
        }

        // 감정별 사운드 재생
        PlayEmotionSound(emotion.dominantEmotion);
    }

    private void HandleKeywords(List<string> keywords)
    {
        foreach (string keyword in keywords)
        {
            Debug.Log($"키워드 감지: {keyword}");
            // 키워드별 특수 효과 트리거
            TriggerKeywordEffect(keyword);
        }
    }

    private void HandleVoiceResult(VoiceRecognitionResult result)
    {
        // 음성 인식 결과를 UI에 표시
        DisplayVoiceResult(result.transcription, result.confidence);
    }
}
```

#### 🌐 네트워크 동기화 고급 기능
```csharp
public class AdvancedNetworkedObject : NetworkBehaviour, INetworkBehaviour
{
    [Networked] public Vector3 NetworkPosition { get; set; }
    [Networked] public Quaternion NetworkRotation { get; set; }
    [Networked] public float AnimationParameter { get; set; }
    [Networked, Capacity(10)] public NetworkArray<byte> CustomData { get; }

    // 네트워크 권한 체크
    public bool HasNetworkAuthority => Object.HasInputAuthority;

    // 지연 보상 및 예측
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float networkSendTimer;

    public override void FixedUpdateNetwork()
    {
        if (HasNetworkAuthority)
        {
            // 권한이 있는 클라이언트에서 데이터 송신
            NetworkPosition = transform.position;
            NetworkRotation = transform.rotation;

            // 커스텀 데이터 전송 (예: 애니메이션 상태)
            UpdateCustomNetworkData();
        }
        else
        {
            // 다른 클라이언트의 데이터 수신 및 보간
            ApplyNetworkInterpolation();
        }
    }

    private void ApplyNetworkInterpolation()
    {
        // 부드러운 위치 보간
        transform.position = Vector3.Lerp(transform.position, NetworkPosition, Time.fixedDeltaTime * 20f);
        transform.rotation = Quaternion.Lerp(transform.rotation, NetworkRotation, Time.fixedDeltaTime * 20f);
    }

    // RPC를 통한 특정 이벤트 전송
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void TriggerNetworkEffect(Vector3 position, int effectType)
    {
        // 모든 클라이언트에서 효과 재생
        PlayNetworkEffect(position, effectType);
    }
}
```

#### 🎵 오디오 및 음성 처리
```csharp
public class VoiceRecordingManager : MonoBehaviour
{
    [SerializeField] private float recordingDuration = 5f;
    [SerializeField] private int sampleRate = 44100;
    private AudioClip recordedClip;
    private bool isRecording;

    // WebSocket 클라이언트 참조
    private WebSocketVoiceClient voiceClient;

    void Start()
    {
        voiceClient = FindObjectOfType<WebSocketVoiceClient>();
    }

    public void StartRecording()
    {
        if (!isRecording && Microphone.devices.Length > 0)
        {
            string micDevice = Microphone.devices[0];
            recordedClip = Microphone.Start(micDevice, false, (int)recordingDuration, sampleRate);
            isRecording = true;

            StartCoroutine(RecordingCoroutine());
        }
    }

    private IEnumerator RecordingCoroutine()
    {
        yield return new WaitForSeconds(recordingDuration);

        if (isRecording)
        {
            StopRecording();
        }
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;

            // 녹음된 오디오를 AI 서버로 전송
            if (voiceClient != null && recordedClip != null)
            {
                byte[] audioData = WavUtility.FromAudioClip(recordedClip);
                voiceClient.SendAudioData(audioData);
            }
        }
    }

    // 실시간 오디오 레벨 모니터링
    public float GetAudioLevel()
    {
        if (!isRecording || recordedClip == null) return 0f;

        float[] samples = new float[256];
        int micPosition = Microphone.GetPosition(null);
        recordedClip.GetData(samples, micPosition - 256);

        float sum = 0f;
        foreach (float sample in samples)
        {
            sum += Mathf.Abs(sample);
        }

        return sum / samples.Length;
    }
}
```

### 🛠️ 커스터마이징 가이드

#### 새로운 제스처 추가
```csharp
// 1. 새로운 제스처 데이터 생성
// Assets/Hand Shapes/ 폴더에서 우클릭 → Create → XR → Hand Gesture

// 2. 제스처 설정 스크립트
[CreateAssetMenu(fileName = "NewGesture", menuName = "VStage/Hand Gesture")]
public class CustomHandGesture : StaticHandGesture
{
    [Header("커스텀 설정")]
    public float activationThreshold = 0.8f;
    public float holdDuration = 0.5f;

    protected override void OnValidate()
    {
        base.OnValidate();
        // 커스텀 유효성 검증
    }
}

// 3. 제스처 핸들러에 등록
public class GestureManager : MonoBehaviour
{
    [SerializeField] private List<CustomHandGesture> availableGestures;

    void Start()
    {
        foreach (var gesture in availableGestures)
        {
            gesture.GesturePerformed.AddListener(() => OnGestureDetected(gesture));
        }
    }

    private void OnGestureDetected(CustomHandGesture gesture)
    {
        // 제스처별 처리 로직
        HandleGestureAction(gesture.name);
    }
}
```

#### AI 분석 기능 확장
```csharp
// 1. 새로운 데이터 타입 정의
[System.Serializable]
public class CustomAIData
{
    public string dataType;
    public float confidence;
    public Dictionary<string, object> metadata;
    public long timestamp;
}

// 2. WebSocket 클라이언트 확장
public class ExtendedWebSocketClient : WebSocketVoiceClient
{
    public UnityEvent<CustomAIData> OnCustomDataReceived;

    protected override void ProcessReceivedData(string jsonData)
    {
        base.ProcessReceivedData(jsonData);

        // 커스텀 데이터 타입 처리
        if (jsonData.Contains("\"type\":\"custom\""))
        {
            CustomAIData customData = JsonUtility.FromJson<CustomAIData>(jsonData);
            OnCustomDataReceived?.Invoke(customData);
        }
    }

    public void SendCustomData(object data)
    {
        string jsonData = JsonUtility.ToJson(new
        {
            type = "custom_input",
            data = data,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });

        SendData(jsonData);
    }
}

// 3. 확장된 데이터 저장소
public class ExtendedAIResponseStore : AIResponseStore
{
    [Header("확장 기능")]
    public UnityEvent<CustomAIData> OnCustomAIDataReceived;

    private Queue<CustomAIData> customDataHistory = new Queue<CustomAIData>();

    public void StoreCustomData(CustomAIData data)
    {
        customDataHistory.Enqueue(data);

        // 최대 100개 항목만 유지
        if (customDataHistory.Count > 100)
        {
            customDataHistory.Dequeue();
        }

        OnCustomAIDataReceived?.Invoke(data);
    }

    public CustomAIData GetLatestCustomData()
    {
        return customDataHistory.Count > 0 ? customDataHistory.Peek() : null;
    }
}
```

#### 성능 최적화 도구
```csharp
public class VRPerformanceOptimizer : MonoBehaviour
{
    [Header("성능 설정")]
    public int targetFrameRate = 72; // Quest 3 권장 프레임레이트
    public bool enableFixedFoveatedRendering = true;
    public bool enableDynamicResolution = true;

    void Start()
    {
        OptimizeForQuest3();
    }

    private void OptimizeForQuest3()
    {
        // 고정 프레임레이트 설정
        Application.targetFrameRate = targetFrameRate;
        QualitySettings.vSyncCount = 0;

        // Oculus 특화 최적화
        if (OVRManager.instance != null)
        {
            // 고정 포비에이티드 렌더링 활성화
            if (enableFixedFoveatedRendering)
            {
                OVRManager.fixedFoveatedRenderingLevel = OVRManager.FixedFoveatedRenderingLevel.High;
            }

            // 동적 해상도 스케일링
            if (enableDynamicResolution)
            {
                OVRManager.instance.enableAdaptiveGpuPerformanceScale = true;
            }
        }

        // 메모리 관리
        StartCoroutine(MemoryCleanupRoutine());
    }

    private IEnumerator MemoryCleanupRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(30f);

            // 정기적인 가비지 컬렉션
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
        }
    }

    // 실시간 성능 모니터링
    void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"FPS: {1f / Time.deltaTime:F1}");
            GUI.Label(new Rect(10, 30, 300, 20), $"Memory: {System.GC.GetTotalMemory(false) / 1024 / 1024} MB");
        }
    }
}
```

## 🐛 문제 해결

### 🔧 자주 발생하는 문제

#### 1. 핸드 트래킹이 작동하지 않음
```
증상: 제스처 인식이 되지 않거나 손이 인식되지 않음

해결책:
✅ Quest 3 설정에서 핸드 트래킹 활성화 확인
✅ 조명이 충분한 환경에서 사용 (어두운 곳에서는 인식률 저하)
✅ Unity XR Hands 패키지 버전 확인 (1.5.1+)
✅ 손목이 완전히 보이도록 소매를 올려서 착용
✅ 반지나 장갑 등 손 인식을 방해하는 요소 제거

디버그 방법:
- Unity Console에서 XR Hands 관련 로그 확인
- StaticHandGesture 컴포넌트의 Confidence 값 모니터링
```

#### 2. Host-Client 네트워킹 연결 실패
```
증상: Host 방에 입장할 수 없거나 연결이 끊어짐

Host 측 점검:
✅ Host PC에서 V-Stage Win 애플리케이션이 정상 실행 중인지 확인
✅ Photon Fusion 방이 생성되었는지 확인
✅ Windows 방화벽에서 Unity 및 Photon 관련 포트 허용
✅ VR 트래커가 정상적으로 연결되어 있는지 확인

Client 측 해결책:
✅ Quest 3의 Wi-Fi 연결 상태 확인 (5GHz 네트워크 권장)
✅ Photon AppId가 Host와 동일한지 확인
✅ 네트워크 지연시간 확인 (ping 100ms 이하 권장)
✅ Quest 3 재시작 후 재연결 시도

디버그 방법:
- Photon Statistics GUI 활성화
- Network Manager의 연결 상태 로그 확인
```

#### 3. AI 음성 분석 서버 연결 문제
```
증상: 음성 녹음은 되지만 AI 분석 결과가 오지 않음

해결책:
✅ WebSocket 서버 주소와 포트 확인
✅ Quest 3의 마이크 권한 허용 확인
✅ AI 서버가 실행 중인지 확인
✅ 네트워크 방화벽에서 WebSocket 포트 허용
✅ 오디오 포맷 및 샘플레이트 설정 확인 (44.1kHz 권장)

디버그 방법:
- WebSocketVoiceClient의 연결 상태 로그 확인
- Chrome DevTools로 WebSocket 연결 테스트
- AI 서버 로그에서 수신된 오디오 데이터 확인
```

#### 4. VR 아바타 동기화 문제
```
증상: 다른 사용자의 아바타 움직임이 부자연스럽거나 지연됨

해결책:
✅ 네트워크 지연시간 최적화 (라우터와 가까운 위치에서 사용)
✅ Photon Fusion의 Tick Rate 설정 확인 (60Hz 권장)
✅ Final IK 컴포넌트 설정 최적화
✅ 네트워크 보간 설정 조정

디버그 방법:
- Network Transform의 동기화 설정 확인
- Photon Statistics에서 네트워크 성능 모니터링
```

#### 5. Quest 3 빌드 및 설치 문제
```
증상: APK 빌드 실패 또는 설치되지 않음

해결책:
✅ Android Build Tools 최신 버전 설치 확인
✅ Unity Android 모듈이 설치되어 있는지 확인
✅ Quest 3 개발자 모드 및 USB 디버깅 활성화
✅ USB 케이블 연결 상태 확인 (데이터 전송 지원 케이블 사용)
✅ Quest 3 저장 공간 확인 (최소 2GB 여유 공간)

Gradle 빌드 오류 시:
- Unity Preferences → External Tools에서 Android SDK 경로 확인
- Build Settings → Player Settings → Publishing Settings에서 Keystore 설정 확인
```

#### 6. 성능 최적화 문제
```
증상: Quest 3에서 프레임 드랍이나 버벅거림 발생

해결책:
✅ VRPerformanceOptimizer 컴포넌트 사용
✅ Fixed Foveated Rendering 활성화
✅ 동적 해상도 스케일링 사용
✅ 불필요한 파티클 시스템 및 이펙트 최소화
✅ 텍스처 압축 설정 최적화 (ASTC 포맷 사용)

모니터링:
- OVR Metrics Tool 사용하여 실시간 성능 확인
- Unity Profiler로 병목 지점 분석
```

### 🛠️ 고급 디버깅 도구

#### Photon 네트워킹 디버그
```csharp
public class NetworkDebugger : MonoBehaviour
{
    void OnGUI()
    {
        if (Debug.isDebugBuild)
        {
            GUILayout.BeginVertical();
            GUILayout.Label($"Connection State: {NetworkRunner.IsClient}");
            GUILayout.Label($"Players: {NetworkRunner.ActivePlayers.Count()}");
            GUILayout.Label($"Ping: {NetworkRunner.GetPlayerRtt(NetworkRunner.LocalPlayer)}ms");
            GUILayout.EndVertical();
        }
    }
}
```

#### AI 서버 연결 테스트
```csharp
public class AIServerTester : MonoBehaviour
{
    [SerializeField] private string serverUrl = "ws://localhost:8080";

    [ContextMenu("Test Connection")]
    public void TestConnection()
    {
        StartCoroutine(TestWebSocketConnection());
    }

    private IEnumerator TestWebSocketConnection()
    {
        // WebSocket 연결 테스트 로직
        Debug.Log($"Testing connection to: {serverUrl}");
    }
}
```


### 프로파일링
```csharp
// 성능 모니터링을 위한 유틸리티 사용
TargetFPS targetFPS = FindObjectOfType<TargetFPS>();
targetFPS.SetTargetFrameRate(72); // Quest 3 권장 프레임레이트
```

## 🤝 기여하기

1. Fork 프로젝트
2. Feature 브랜치 생성 (`git checkout -b feature/AmazingFeature`)
3. 변경사항 커밋 (`git commit -m 'Add some AmazingFeature'`)
4. 브랜치 Push (`git push origin feature/AmazingFeature`)
5. Pull Request 열기

### 코딩 스타일
- C# 네이밍 컨벤션 준수 (PascalCase for public, camelCase for private)
- XML 문서화 주석 작성
- Unity Inspector 친화적인 SerializeField 사용

## 📄 라이선스

이 프로젝트는 [LICENSE](LICENSE) 파일에 명시된 라이선스 하에 제공됩니다.

## 🙏 감사의 말

### 사용된 오픈소스 프로젝트
- [Unity Technologies](https://unity.com/) - 게임 엔진
- [Photon Engine](https://www.photonengine.com/) - 네트워킹 솔루션
- [Meta](https://www.meta.com/) - XR 플랫폼 및 SDK
- [MagicaCloth2](https://assetstore.unity.com/packages/tools/physics/magica-cloth-2-242307) - 의상 물리 시뮬레이션
- [Final IK](https://assetstore.unity.com/packages/tools/animation/final-ik-14290) - IK 솔루션

### 특별 감사
- Unity XR 팀 - XR Hands 패키지 개발
- Photon 팀 - Fusion 네트워킹 엔진
- Meta Reality Labs - Quest 3 플랫폼 지원

---

## 📞 연락처 및 지원

**V-Stage 프로젝트 정보**
- **Client (이 프로젝트)**: V-Stage Quest3 Public
  - 버전: 1.0.0
  - Unity 버전: 2023.3.0f1+
  - 플랫폼: Meta Quest 3
  - 용도: VR 콘서트 관객용 애플리케이션

- **Host (별도 프로젝트)**: [V-Stage Win Public](https://github.com/carlton368/vstage_win_public)
  - 버전: 1.0.0
  - Unity 버전: 6000.1.2f1+
  - 플랫폼: Windows + VR Trackers
  - 용도: VR 콘서트 공연자용 애플리케이션

**기술 지원**
- **Client Issues**: [GitHub Issues](https://github.com/your-repo/vstage_quest3_public/issues)
- **Host Issues**: [V-Stage Win Public Issues](https://github.com/carlton368/vstage_win_public/issues)
- **통합 문서**: 각 프로젝트의 README 및 코드 내 주석 참조
- **Unity 호환성**:
  - Client: Unity 2023.3 LTS 권장
  - Host: Unity 6000.1.2f1+ 권장

**시스템 요구사항 요약**
- **최소 사용자**: Host 1명 + Client 1명
- **권장 사용자**: Host 1명 + Client 최대 20명 (네트워크 성능에 따라)
- **필수 서버**: AI 분석 서버 (WebSocket)
- **네트워크**: 안정적인 LAN 또는 고속 인터넷 연결

---

<div align="center">

**🎭 V-Stage Quest3에서 차세대 VR 콘서트를 경험해보세요! 🎵**

*Made with ❤️ for the VR Community*

</div>
