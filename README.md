# 2D 플랫포머 게임 (Unity)
<br/>

## - 애니메이션
<img src="https://user-images.githubusercontent.com/86781939/235141631-4cfe7876-e484-47be-b238-da2f02a6e0e5.png"  width="1200" height="540" > <img src="https://user-images.githubusercontent.com/86781939/235142481-63facdec-1818-40e6-a292-2f4d3e446b8c.png"  width="1200" height="540" >

   1. 조이스틱 이동
```python
float x = joy.Horizontal;
rb.velocity = new Vector2(x * playerSpeed, rb.velocity.y);
```
   2. 점프 (A)
```python
rb.velocity = Vector2.up * jumpPower;
```
   3. 공격 (B)
```python
GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
bullet_rb.AddForce(Vector2.right * 10, ForceMode2D.Impulse);
```
<br/>

## - 스테이지

  ### 1. 스테이지 1
  - 이동 (추적)
  - 점프
  - 공격
  
  ### 2. 스테이지 2
  - 이동 (추적)
  - 공격
  - 스킬 (돌진)
  
  ### 3. 스테이지 3
  - 이동 (추적)
  - 공격
  - 스킬1 (몬스터 소환)
  - 스킬2 (탄환 발사)

## - 문제 해결
   - AddForce 적용 안됨 -> AddForce 사용 시점에 Rigidbody의 Velocity를 초기화해야 작동.
   - 오브젝트간 충돌 발생 -> 레이어 만들어 프로젝트 세팅에서 특정 레이어간 충돌 비활성화
<br/>

## - 사용 에셋
  - 폰트 (https://assetstore.unity.com/packages/2d/fonts/free-pixel-font-thaleah-140059)
  - 조이스틱 팩 (https://assetstore.unity.com/packages/tools/input-management/joystick-pack-107631)
  - 캐릭터, 배경(타일맵), 파티클 (https://www.kenney.nl/assets)
<br/>

<img src="https://img.shields.io/badge/Unity-212121?style=for-the-badge&logo=Unity&logoColor=white"><img src="https://img.shields.io/badge/Visual%20Studio-5C2D91?style=for-the-badge&logo=Visual%20Studio&logoColor=white"><img src="https://img.shields.io/badge/C%20Sharp-239120?style=for-the-badge&logo=C%20Sharp&logoColor=white"><img src="https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=GitHub&logoColor=white">
