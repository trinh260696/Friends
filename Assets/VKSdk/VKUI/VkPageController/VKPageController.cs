using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace VKSdk.UI
{
    // page run from 1
    public class VKPageController : MonoBehaviour
    {
        public Text[] txtPages;
        public VKButton[] btPages;

        public VKButton btBackAll;
        public VKButton btBack;
        public VKButton btNext;
        public VKButton btNextAll;

        public int currentPage;
        public int maxPage;

        public Color cNormalPage;
        public Color cCurrentPage;

        public Action<int> OnSelectPage;

        private int[] numberPages;
        private int countBtPage;

        public void ButtonBackAllClickListener()
        {
            currentPage = 1;
            SetupButton();
            SetupNumberPage();

            if (OnSelectPage != null)
                OnSelectPage(currentPage);
        }

        public void ButtonBackClickListener()
        {
            currentPage--;
            SetupButton();
            SetupNumberPage();

            if (OnSelectPage != null)
                OnSelectPage(currentPage);
        }

        public void ButtonNextAllClickListener()
        {
            currentPage = maxPage;
            SetupButton();
            SetupNumberPage();

            if (OnSelectPage != null)
                OnSelectPage(currentPage);
        }

        public void ButtonNextClickListener()
        {
            currentPage++;
            SetupButton();
            SetupNumberPage();

            if (OnSelectPage != null)
                OnSelectPage(currentPage);
        }

        public void ButtonPageClickListener(int index)
        {
            if (currentPage == numberPages[index])
                return;

            currentPage = numberPages[index];
            SetupButton();
            SetupNumberPage();

            if (OnSelectPage != null)
                OnSelectPage(currentPage);
        }

        public void InitPage(int maxPage, Action<int> onSelectPage)
        {
            countBtPage = btPages.Length;
            numberPages = Enumerable.Range(1, countBtPage).ToArray();
            if (maxPage > 0)
                currentPage = 1;
            else
                currentPage = 0;
            this.maxPage = maxPage;
            this.OnSelectPage = onSelectPage;

            SetupButton();
            SetupTextPage();
        }

        private void SetupButton()
        {
            btBack.VKInteractable = currentPage > 1;
            btBackAll.VKInteractable = currentPage > 2;
            btNext.VKInteractable = currentPage < maxPage;
            btNextAll.VKInteractable = maxPage > 2 && currentPage < maxPage - 1;

            for(int i = 0; i < countBtPage; i++)
            {
                btPages[i].VKInteractable = maxPage >= numberPages[i];
            }
        }

        private void SetupNumberPage()
        {
            int index = Array.IndexOf(numberPages, currentPage);
            if (index == countBtPage - 1 && maxPage > currentPage)
            {
                numberPages = Enumerable.Range(currentPage + 2 - countBtPage, countBtPage).ToArray();
            }
            else if (index == 0 && currentPage > 1)
            {
                numberPages = Enumerable.Range(currentPage - 1, countBtPage).ToArray();
            }
            else if (currentPage == 1 || (currentPage < countBtPage && maxPage <= countBtPage))
            {
                numberPages = Enumerable.Range(1, countBtPage).ToArray();
            }
            else if (currentPage == maxPage)
            {
                numberPages = Enumerable.Range(maxPage - countBtPage + 1, countBtPage).ToArray();
            }
            SetupTextPage();
        }

        private void SetupTextPage()
        {
            for (int i = 0; i < countBtPage; i++)
            {
                txtPages[i].text = numberPages[i].ToString();
                txtPages[i].color = currentPage == numberPages[i] ? cCurrentPage : cNormalPage;
            }

            for(int i = 0; i < countBtPage; i++)
            {
                if (!btPages[i].VKInteractable)
                    btPages[i].SetupAll(btPages[i].VKInteractable);
            }
        }
    }
}