  
  
  
  <div align="center">

# HolyKnightsVSDarkLegion

 <div align="center">
  <img src="https://github.com/Dalsi-0/HolyKnightsVSDarkLegion/blob/main/Readme/MainImage.png?raw=true" width="350"/>
</div>

<br/> [<img src="https://img.shields.io/badge/프로젝트 기간-2025.03.27~2025.04.02-73abf0?style=flat&logo=&logoColor=white" />]()

</div> 

---

## 📌 프로젝트 개요  

- **프로젝트명** : HolyKnights VS DarkLegion (성기사 대 암흑군단)
- **개발환경** : C#, Unity  
- **프로젝트 기간** : 2025.03.27 ~ 2025.04.02  
- **개발 인원** : 20조 5명  
  - 김자은, 정수영, 김효중, 양원준, 최상준 

---
## 🎮 게임 소개  

> **HolyKnights VS DarkLegion** 은 식물 vs 좀비를 레퍼런스로 만든 **2D 타워 디펜스 게임**입니다.  
> 플레이어는 **유닛을 배치**하며 공격해오는 적들을 막아야합니다.  
> 전략적인 **유닛 배치와 자원 관리**가 핵심 요소이며, 웨이브 방식으로 점점 강해지는 적들을 상대하게 됩니다.  
> 게임은 **간단한 조작 방식**과 **직관적인 플레이 흐름**을 제공하며, 다양한 유닛을 활용하여 적을 막아내는 재미를 제공합니다.  


---
## 👥 팀원 소개 및 업무 분담  

| 이름 | 역할 | 업무 상세내용 | GitHub 링크 |
|------|------|------------|---|
| 김자은 | 팀장 | 몬스터 AI <br> 몬스터 및 스테이지 효과음 |  https://github.com/Cindy0914 |
| 정수영 | 팀원 | 맵 디자인 <br> 씬 전환 기능 <br> 사운드 기능 |    https://github.com/JungSuYoung-98 |
| 김효중 | 팀원 | 덱 정보 저장 및 불러오기 <br> 카드로 유닛 배치 기능 <br> 툴팁, 획득 UI |    https://github.com/hn7093 |
| 양원준 | 팀원 | 플레이어 유닛 제작 <br> 유닛 밸런싱 |    https://github.com/kierwl |
| 최상준 | 팀원 | 스테이지 진행 <br> 몬스터 소환 <br> 데이터 테이블 관련 기능 |    https://github.com/Dalsi-0 |


---

## 🎮 주요 기능  

- **전투 시스템** : 유닛 배치 및 근거리, 원거리, 특수 유닛 전투 구현
- **레벨 디자인** : 랜덤 라운드 진행 및 적의 특수 능력 적용
- **UI 및 UX** : 소지 자원 및 라운드 진행 상황 표시
  
- **플레이어 스킬** : 애니메이션 이벤트를 활용한 쿨타임 시스템 구현
- **Grid Sensor** : 물리 충돌이 아닌 그리드 데이터 기반으로 몬스터 제어
- **Debuff Handler** : 디버프 데이터 인터페이스를 활용하여 효과 적용 및 관리
- **데이터 테이블 다운로드 툴** : EditorWindow를 활용한 커스텀 창 생성 및 OnGUI를 통한 버튼 인터페이스 구성
- **몬스터 소환** : 팩토리 패턴을 이용한 몬스터 생성 및 오브젝트 풀링을 통한 메모리 최적화
- **스테이지 흐름 관리** : 상태 패턴을 활용하여 적 스폰 및 승리·패배 상황 관리
- **플레이어 유닛 배치** : 월드 좌표 ↔ 그리드 중심 좌표 변환을 통해 정교한 유닛 배치 시스템 구현

---

## 📸 스크린샷
|메인 화면|
|:---:|
|<img src="https://github.com/Dalsi-0/HolyKnightsVSDarkLegion/blob/main/Readme/Thumbnail.png?raw=true?raw=true?raw=true" width="700"/>|
|<img src="https://github.com/Dalsi-0/HolyKnightsVSDarkLegion/blob/main/Readme/option.png?raw=true?raw=true?raw=true?raw=true" width="700"/>|
|게임 시작 및 설정 화면입니다.|

<br /><br />

|게임 플레이 장면|
|:---:|
|<img src="https://github.com/Dalsi-0/HolyKnightsVSDarkLegion/blob/main/Readme/MainImage.png?raw=true?raw=true" width="700"/>|
|플레이어가 유닛을 배치하고 적의 공격을 방어하는 장면입니다.|  

<br /><br />

|로딩|
|:---:|
|<img src="https://github.com/Dalsi-0/HolyKnightsVSDarkLegion/blob/main/Readme/newLoading.png?raw=true?raw=true" width="700"/>|
|로딩 화면에서 게임 플레이 가이드 영상을 재생하는 장면입니다.|

<br /><br />

|덱 편집|
|:---:|
|<img src="https://github.com/Dalsi-0/HolyKnightsVSDarkLegion/blob/main/Readme/Deck.png?raw=true?raw=true" width="700"/>|
|플레이어가 유닛 카드를 선택하여 덱을 편집하는 장면입니다. |


---

## 🛠 **개발 및 기술적 접근**

### 📊 데이터 연동 – Google Spreadsheet  
이 프로젝트에서는 Google Spreadsheet를 이용하여 스트레스 진행에 사용되는 대화 데이터를 관리하고, 이를 코드에서 불러와 활용합니다.

**Google Sheets To Unity 에셋 활용**  
Google Sheets To Unity 에셋을 사용하여 Google Sheet의 데이터를 JSON으로 변환하는 과정 없이 바로 Unity에서 사용할 데이터로 가공할 수 있도록 구현하였습니다.  
  
    
**사용된 스프레드시트 데이터**
- 대화 데이터 [(보기)](https://docs.google.com/spreadsheets/d/1M_9bD-lXn9BubHhOxKe_PDYnQMD7g540m2OHiKCczQs/edit?gid=0#gid=0)
  
| 사용 코드 | Scriptable Object |
|---|---|
|<img src="https://github.com/Dalsi-0/Factory404/blob/main/Readme/googleScript.png?raw=true" width="500"/>|<img src="https://github.com/Dalsi-0/Factory404/blob/main/Readme/googleSO.png?raw=true" width="500"/>|

<br /><br />

### 🏃 그리드 기반 몬스터 센서 시스템
기존의 물리 충돌 방식(Trigger) 대신 UnitManager의 그리드 데이터를 기반으로 몬스터 감지 시스템을 구현하였습니다.
몬스터는 자신의 현재 위치(currentCell)와 앞 칸(frontCell)의 상태를 지속적으로 체크하여 유닛이 존재하는 경우에만 공격 상태로 전환됩니다.
이를 통해 물리 충돌 연산을 최소화하고, 퍼포먼스를 최적화할 수 있었습니다.

🔹 최적화 요소
✅ Update() 대신 0.01초 간격의 Coroutine으로 체크 루프 구성 → 불필요한 연산 감소
✅ IAttackRangeCalc 인터페이스 기반의 범위 계산 시스템 적용 → 몬스터별 범위 공격을 유연하게 처리

|몬스터의 그리드 기반 감지 시스템|
|:---:|
|<img src="https://github.com/Dalsi-0/Factory404/blob/main/Readme/Slope.png?raw=true" width="500"/>|

<br /><br />

### ❄ 디버프 전용 핸들러 시스템 (MonsterDebuffHandler)
몬스터의 상태이상 처리를 전담하는 MonsterDebuffHandler 클래스를 구성하여 책임 분리(SRP 준수) 및 코드 가독성을 개선하였습니다.
모든 디버프는 DebuffData 클래스로 추상화하여 타입별 분기 없이 적용이 가능하도록 설계되었습니다.

🔹 최적화 요소
✅ 코루틴 기반의 비동기 처리로 시간 기반 효과를 자연스럽게 관리
✅ Dictionary<DEBUFF_TYPE, Coroutine>을 사용하여 중복 적용 방지
✅ Action 델리게이트 활용으로 디버프 종료 시점의 클린한 처리

|몬스터 디버프 적용 화면|
|:---:|
|<img src="https://github.com/Dalsi-0/Factory404/blob/main/Readme/Slope.png?raw=true" width="500"/>|

<br /><br />

### 🔄 확장성 중심의 인터페이스 기반 구조
게임 내 다양한 기능을 확장 가능하도록 인터페이스 기반의 유연한 설계를 적용하였습니다.
예를 들어, 공격 방식(IAttackRangeCalc)과 디버프(DebuffData) 등을 인터페이스화하여, 조건 분기(switch, if)를 최소화하고 새로운 타입을 쉽게 추가할 수 있도록 구현했습니다.

✅ 예시: 세로 공격/가로 공격/십자 공격 등은 새로운 클래스만 추가하면 자동 적용

|인터페이스 기반의 확장 가능 구조|
|:---:|
|<img src="https://github.com/Dalsi-0/Factory404/blob/main/Readme/Slope.png?raw=true" width="500"/>|

<br /><br />

### 💾 Application.persistentDataPath와 JSON을 이용한 저장, 불러오기
게임 데이터의 저장 및 불러오기를 위해 Application.persistentDataPath와 JSON 직렬화(Serialization) 방식을 활용하였습니다. 이를 통해 게임 종료 후에도 플레이어의 진행 상태를 유지할 수 있으며, 플랫폼에 관계없이 안정적인 데이터 관리를 할 수 있습니다.

🔹 구현 방식
✅ Application.persistentDataPath를 사용하여 플랫폼별 저장 경로를 자동 지정
✅ JSON 직렬화를 활용하여 데이터를 읽고 쓸 때 가독성을 유지
✅ **파일 시스템(IO)**을 최소화하기 위해 데이터 변경 시점에만 저장

🔹 저장 데이터 예시
```
{
  "playerLevel": 10,
  "highScore": 25000,
  "settings": {
    "bgmVolume": 0.8,
    "sfxVolume": 0.7
  }
}
```

🔹 최적화 요소
✅ 게임 시작 시 비동기 로드(Async Load) 적용 → 로딩 속도 향상
✅ 저장 주기를 조정하여 불필요한 디스크 접근 최소화
✅ Dictionary 기반 데이터 관리로 빠른 탐색 및 수정 가능

|데이터 저장 및 불러오기 구조|
|:---:|
|<img src="https://github.com/Dalsi-0/Factory404/blob/main/Readme/Slope.png?raw=true" width="500"/>|

<br /><br />


---

## 🕹️ 플레이 링크  
**👉 [플레이하기](https://play.unity.com/ko/games/a0a535b5-e4c1-4461-a47a-85ff9ddc130a/holyknights-vs-darklegion)**

웹 브라우저에서 바로 플레이할 수 있습니다! 🎮   

---

## 📹 플레이 영상

**[![유튜브](https://github.com/Dalsi-0/HolyKnightsVSDarkLegion/blob/main/Readme/Thumbnail.png?raw=true?raw=true)](https://youtu.be/YG0Hfr0dvWE)** 

---