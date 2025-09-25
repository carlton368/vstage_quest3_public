# V-Stage Quest3 Public 🎭

**차세대 AI 기반 실시간 버츄얼 콘서트 플랫폼 - Meta Quest3 관객용**

[![Unity](https://img.shields.io/badge/Unity-2023.3+-000000.svg?logo=unity&style=flat)](https://unity.com/)
[![Meta Quest 3](https://img.shields.io/badge/Meta_Quest_3-Compatible-blue.svg)](https://www.meta.com/quest/quest-3/)
[![Photon Fusion](https://img.shields.io/badge/Photon_Fusion-Networking-green.svg)](https://www.photonengine.com/fusion)
[![XR Hands](https://img.shields.io/badge/Unity_XR_Hands-Tracking-orange.svg)](https://docs.unity3d.com/Packages/com.unity.xr.hands@1.5/manual/index.html)

## 📖 개요

V-Stage Quest3는 메타버스 환경에서 실시간 VR 콘서트를 경험할 수 있는 혁신적인 플랫폼입니다. AI 기반 실시간 감정 분석과 멀티플레이어 네트워킹을 통해 관객들이 능동적으로 참여할 수 있는 몰입형 버츄얼 콘서트 경험을 제공합니다.

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

### 하드웨어
- **VR 헤드셋**: Meta Quest 3 (필수)
- **PC**: Windows 10/11 (Unity Editor용)
- **RAM**: 16GB 이상 권장
- **GPU**: GTX 1660 이상 권장

### 소프트웨어
- **Unity**: 2023.3.0f1 이상
- **Unity XR Plugin Management**: 4.4.1+
- **Meta XR SDK**: 68.0.0+

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

### 네트워크 구조
```
호스트 (VR Performer)
    ↓ [Photon Fusion]
관객들 (Quest3 Users)
    ↓ [WebSocket]
AI 분석 서버
```

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

### 1. 프로젝트 설정
```bash
# Unity Hub에서 Unity 2023.3.0f1+ 설치
# 프로젝트 폴더를 Unity에서 열기
# Package Manager에서 필요한 패키지 자동 설치 확인
```

### 2. Meta Quest 3 설정
```bash
# Meta Quest Developer Hub 설치
# Quest 3를 개발자 모드로 설정
# USB 디버깅 활성화
```

### 3. 빌드 설정
```bash
# File → Build Settings
# Platform: Android
# XR Settings: Oculus 체크
# Minimum API Level: Android 10.0 (API 29)
```

### 4. 실행
```bash
# Unity Editor에서 Play 또는
# Quest 3에 직접 빌드 및 설치
```

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
   - 👏 박수 제스처 → 응원 효과
   - 🤏 라이트스틱 잡기 → 응원봉 조작

### 호스트 모드 (VR 트래커 필요)
1. **VR 트래커 설정** 및 캘리브레이션
2. **네트워크 방 생성**
3. **타임라인 재생**으로 공연 시작
4. **실시간 아바타 제어**

## 🔧 개발자 가이드

### 주요 API

#### 제스처 인식
```csharp
public class CustomGestureHandler : MonoBehaviour
{
    [SerializeField] private StaticHandGesture gestureToDetect;

    void OnEnable()
    {
        gestureToDetect.GesturePerformed.AddListener(OnGestureDetected);
    }

    private void OnGestureDetected(StaticHandGesture gesture)
    {
        Debug.Log($"제스처 감지: {gesture.name}");
    }
}
```

#### AI 분석 결과 사용
```csharp
public class EmotionReactor : MonoBehaviour
{
    void Start()
    {
        AIResponseStore.Instance.OnKeywordDataUpdated.AddListener(HandleKeywords);
        AIResponseStore.Instance.OnEmotionDataUpdated.AddListener(HandleEmotion);
    }

    private void HandleEmotion(EmotionData emotion)
    {
        Color emotionColor = EmotionColorMapper.GetColor(emotion.dominantEmotion);
        // 감정에 따른 시각적 효과 적용
    }
}
```

#### 네트워크 동기화
```csharp
public class NetworkedObject : NetworkBehaviour
{
    [Networked] public Vector3 NetworkPosition { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            NetworkPosition = transform.position;
        }
        else
        {
            transform.position = NetworkPosition;
        }
    }
}
```

### 커스터마이징 가이드

#### 새로운 제스처 추가
1. `Assets/Hand Shapes/`에 새 제스처 데이터 생성
2. `StaticHandGesture` 스크립터블 오브젝트 설정
3. 제스처 핸들러 스크립트에 등록

#### AI 분석 기능 확장
1. `WebSocketVoiceClient.cs`에서 새로운 데이터 타입 정의
2. `AIResponseStore.cs`에 저장 로직 추가
3. UI 컴포넌트에서 결과 표시

## 🐛 문제 해결

### 자주 발생하는 문제

#### 1. 핸드 트래킹이 작동하지 않음
```
해결책:
- Quest 3 설정에서 핸드 트래킹 활성화 확인
- 조명이 충분한 환경에서 사용
- Unity XR Hands 패키지 버전 확인 (1.5.1+)
```

#### 2. 네트워킹 연결 실패
```
해결책:
- 방화벽 설정 확인
- Photon AppId 설정 확인
- 네트워크 환경 (WiFi) 안정성 점검
```

#### 3. 오디오 녹음 문제
```
해결책:
- 마이크 권한 허용 확인
- AudioSource 컴포넌트 설정 점검
- WebSocket 서버 연결 상태 확인
```

## 📈 성능 최적화

### 권장 설정
- **Render Pipeline**: URP (Universal Render Pipeline)
- **Graphics API**: Vulkan (Android)
- **Multiview**: Enable
- **Fixed Foveated Rendering**: Level 2-3
- **Texture Quality**: Medium (메모리 절약)

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

**프로젝트 정보**
- **이름**: V-Stage Quest3 Public
- **버전**: 1.0.0
- **Unity 버전**: 2023.3.0f1+
- **플랫폼**: Meta Quest 3

**기술 지원**
- 이슈 리포트: [GitHub Issues](https://github.com/your-repo/vstage_quest3_public/issues)
- 문서: 이 README 파일 및 코드 내 주석 참조
- Unity 버전 호환성: Unity 2023.3 LTS 권장

---

<div align="center">

**🎭 V-Stage Quest3에서 차세대 VR 콘서트를 경험해보세요! 🎵**

*Made with ❤️ for the VR Community*

</div>