# 2D 플랫포머 게임 (Unity)
<br/>

## - 플레이어 조작
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
<img src="https://user-images.githubusercontent.com/86781939/235149247-28f17a67-6303-46f4-bede-973054d1e373.gif"  width="1200" height="540" >
- 이동 (추적)
```python
this.transform.position = new Vector2(transform.position.x + ((target.transform.position - transform.position).normalized).x * moveSpeed * Time.deltaTime, transform.position.y);
```
- 점프
- 공격
```python
GameObject slashEffect = Instantiate(effectPrefab_slash, new Vector3(transform.position.x, transform.position.y, -3), transform.rotation);
```
<br/>
  
### 2. 스테이지 2
- 이동 (추적)
- 공격
- 스킬 (돌진)
<img src="https://user-images.githubusercontent.com/86781939/235149610-9aee0478-9a92-4ddc-991f-a0ba23ca8074.gif"  width="1200" height="540" >
```python
IEnumerator Skill_1()
{
   Vector2 direction = (target.transform.position - transform.position).normalized;

   GameObject chargeEffect = Instantiate(effectPrefab_charge, transform.position, transform.rotation);
   chargeEffect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
   Destroy(chargeEffect, 1.5f);

   yield return new WaitForSeconds(1.0f);
   chargeEffect.GetComponent<Rigidbody2D>().velocity = direction * skillSpeed;
   rb.velocity = direction * skillSpeed;
   yield return new WaitForSeconds(0.5f);
   rb.velocity = Vector3.zero; // velocity 초기화
}
```
<br/>
  
### 3. 스테이지 3
- 이동 (추적)
- 공격
- 스킬1 (몬스터 소환)
<img src="https://user-images.githubusercontent.com/86781939/235149760-496e4150-dffd-4361-9c2e-1db68f594753.gif"  width="1200" height="540" >
```python
Instantiate(minionPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
```
- 스킬2 (탄환 발사)
<img src="https://user-images.githubusercontent.com/86781939/235149767-4f003875-13fe-4682-adf2-7c2e92359de3.gif"  width="1200" height="540" >
```python
IEnumerator Skill_2()
{
   GameObject effect1 = Instantiate(skillPrefab_1, new Vector3(transform.position.x, transform.position.y, -3), transform.rotation);
   effect1.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
   effect1.GetComponent<Rigidbody2D>().velocity = Vector2.up * 5.0f;
   yield return new WaitForSeconds(1f);
   effect1.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

   for (int i = 0;i < 10;i++)
   {
      Vector2 direction = (target.transform.position - effect1.transform.position).normalized;

      GameObject effect2 = Instantiate(skillPrefab_2, effect1.transform.position, Quaternion.AngleAxis((Mathf.Atan2(direction.y, direction.x) + 100) * Mathf.Rad2Deg, Vector3.forward));

      effect2.GetComponent<Rigidbody2D>().velocity = direction * 10.0f;
      effect2.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
      Destroy(effect2, 30.0f);
      yield return new WaitForSeconds(1f);
   }
   Destroy(effect1);
   StartCoroutine(EnableSkill(2, 10.0f));
}
```
<br/>

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
