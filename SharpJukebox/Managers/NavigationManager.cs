using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace SharpJukebox
{
    public class PageNode
    {
        public PageNode NextNode { get; set; }
        public PageNode PreviousNode { get; set; }
        public Page Value { get; set; }

        public PageNode(Page Value)
        {
            this.Value = Value;
        }

    }

    public class NavigationManager
    {
        private PageNode _currentPage;

        public Page CurrentPage { get { return _currentPage?.Value; } }

        public bool PreviousPageAvailable
        {
            get
            {
                if (_currentPage == null || _currentPage.PreviousNode == null)
                    return false;
                return true;
            }
        }
        public bool NextPageAvailable
        {
            get
            {
                if (_currentPage == null || _currentPage.NextNode == null)
                    return false;
                return true;
            }
        }

        public NavigationManager()
        {

        }

        public void NavigateToNewPage(Page NewPage)
        {
            PageNode newPageNode = new PageNode(NewPage);
            if (_currentPage != null)
            {
                _currentPage.NextNode = newPageNode;
                newPageNode.PreviousNode = _currentPage;
            }

            _currentPage = newPageNode;
        }

        public Page PreviousPage()
        {
            if (_currentPage == null || _currentPage.PreviousNode==null)
                return null;
            return _currentPage.PreviousNode.Value;
        }

        public Page NextPage()
        {
            if (_currentPage?.NextNode != null)
                _currentPage = _currentPage.NextNode;
            return _currentPage?.NextNode.Value;
        }

    }
}
