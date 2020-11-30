using System;
using System.Collections.Generic;

namespace MementoPattern
{
    /// <summary>
    /// 手机用户
    /// </summary>
    public class Mobile
    {
        private List<ContactPerson> _contactPersons;

        public List<ContactPerson> GetPhoneBook()
        {
            return _contactPersons;
        }

        public Mobile(List<ContactPerson> contactPersons)
        {
            _contactPersons = contactPersons;
        }

        /// <summary>
        /// 创建备份
        /// </summary>
        /// <returns></returns>
        public ContactMemento CreateMemento()
        {
            //思考以下为什么要new List<ContactPerson>(_contactPersons)
            return new ContactMemento(new List<ContactPerson>(_contactPersons));
        }

        /// <summary>
        /// 恢复备份
        /// </summary>
        /// <param name="memento"></param>
        public void RestoreMemento(ContactMemento memento)
        {
            this._contactPersons = memento.GetMemento();
        }

        public void DisplayPhoneBook()
        {
            Console.WriteLine($"共有{_contactPersons.Count}位联系人，联系人列表如下：");
            foreach (var contactPerson in _contactPersons)
            {
                Console.WriteLine($"姓名：{contactPerson.Name}，电话：{contactPerson.PhoneNumber}");
            }
        }
    }
}