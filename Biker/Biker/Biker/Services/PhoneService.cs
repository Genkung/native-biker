﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Biker.Services
{
    public class PhoneService
    {
        public static void Call(string number)
        {
            try
            {
                PhoneDialer.Open(number);
            }
            catch (ArgumentNullException anEx)
            {
                PageService.DisplayAlert("แจ้งเตือน", "ไม่พบหมายเลขที่ต้องการโทร", "ปิด");
            }
            catch (FeatureNotSupportedException ex)
            {
                PageService.DisplayAlert("แจ้งเตือน", "ไม่สามารถใช้งานระบบโทรศัพย์ได้", "ปิด");
            }
            catch (Exception ex)
            {
                PageService.DisplayAlert("แจ้งเตือน", "ขออภัย เกิดข้อผิดพลาด", "ปิด");
            }
        }
    }
}
