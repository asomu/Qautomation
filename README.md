## Test QPST automatin

## 참고문서

80-PA177-1 RevA

[80-pa177-1_a_qpst_automation_application_notes.pdf](https://www.notion.so/e4f21feece3f4471884d08b71c385440#96fa84521e064e4c9874d67bd98ace3b)

## 개요

QPST API를 이용하여 QPST 기능을 custom 하여 자동화할 수 있다.

## QPST automation sample

### 위치

C:\Program Files (x86)\Qualcomm\QPST\Automation Samples

아래 reaeme file에서 perl script 설명이 있음.

[Automation readme.txt](https://www.notion.so/e4f21feece3f4471884d08b71c385440#7d29747669254294b278fc1256cde01e)

Source

[asomu/Qautomation](https://github.com/asomu/Qautomation)

## Key API

[API Class code](https://www.notion.so/23bea1b272ed4933abe609e548d7e4fc)

1. Add COM Port
2. Remove COM Port
3. Enable/Disable Port
4. Get port 
5. Get portlist
6. Get NV item.
7. restore qcn
8. Send Command
9. Get information
10. Phone reset
11. EFS file remove and write

# 추가 구현

- EFS file 제거, 가져오기, 그리고 정보 읽기
- MIPI 정보, RFSW debug 정보
