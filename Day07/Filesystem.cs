﻿using System.Collections.Generic;
using System.Linq;


namespace Day07
{
    class Filesystem
    {
        private class Node
        {
#nullable enable
            public Node? parent;
            public List<Node> children;
            public string name;
            public ulong? size; //size = null means it is a directory

            public Node(string givenName, ulong? givenSize = null)
            {
                parent = null;
                children = new List<Node>();
                size = givenSize;
                name = givenName;
            }
            public Node(Node givenParent, string givenName, ulong? givenSize = null)
            {
                parent = givenParent;
                children = new List<Node>();
                size = givenSize;
                name = givenName;
                parent.children.Add(this);
            }

            public ulong getSumOfChildrenSize()
            {
                ulong sum = 0;

                foreach(Node child in children)
                {
                    if (child.size is null)
                    {
                        sum += child.getSumOfChildrenSize();
                    }
                    else
                    {
                        sum += (ulong)child.size;
                    }
                }

                return sum;
            }

            //terrible name
            public ulong getSumOfSizeOfDirectoriesSmallerThanSpecifiedSize(ulong maxSize)
            {
                ulong sum = 0;

                ulong sumOfThisDir = this.getSumOfChildrenSize();

                if (sumOfThisDir < maxSize)
                {
                    sum += sumOfThisDir;
                }

                foreach (Node child in children)
                {
                    if (child.size is null)
                    {
                        sum += child.getSumOfSizeOfDirectoriesSmallerThanSpecifiedSize(maxSize);
                    }
                }

                return sum;
            }
        
            public void insertAllSizesBiggerThan(ulong minSize, ref List<ulong> sizes)
            {
                ulong self = getSumOfChildrenSize();

                if (self > minSize)
                {
                    sizes.Add(self);
                    foreach(Node child in children)
                    {
                        child.insertAllSizesBiggerThan(minSize, ref sizes);
                    }
                }
            }
        }

        private Node root;
        private Node currentNode;
        public ulong capacity = 70000000;

        public Filesystem()
        {
            root = new Node("/");
            currentNode = root;
        }

        public void addFile(string name, ulong? size = null)
        {
            new Node(currentNode, name, size); //no, currentNode cannot be null!
        }
        
        public void cd(string name)
        {
            if (name.Equals("/"))
            {
                currentNode = root;
            }
            else if(name.Equals(".."))
            {
                currentNode = currentNode.parent;
            }
            else
            {
                foreach (Node child in currentNode.children)
                {
                    if (child.name.Equals(name))
                    {
                        currentNode = child;
                        break;
                    }
                }
            }
        }
    
        public ulong addDirectoriesSmallerThanSpecifiedSize(ulong maxSize)
        {
            ulong result = 0;

            cd("/");

            result = currentNode.getSumOfSizeOfDirectoriesSmallerThanSpecifiedSize(maxSize);

            return result;
        }

        public ulong getUsedSpace()
        {
            return root.getSumOfChildrenSize();
        }
    
        public ulong findSmallestDirectoryOfMinimalSize(ulong minSize)
        {
            ulong result = 0;

            List<ulong> sizes = new List<ulong>();

            root.insertAllSizesBiggerThan(minSize, ref sizes);

            result = sizes.AsQueryable().Min();

            return result;
        }
    }
}
