import React, { useState, useEffect } from 'react';
import { NavLink } from 'react-router-dom';
import styled from 'styled-components';
import { UserContext, User } from './utils/UserContext';

const LayoutContainer = styled.div`
  margin: 0 auto;
  max-width: 1400px;
  padding-top: 20px;

  @media (max-width: 1440px) {
    padding: 20px 20px 0 20px;
  }

  @media (max-width: 768px) {
    padding: 10px 10px 0 10px;
  }
`;

const MenuWrapper = styled.div`
  background: #c90000;
  overflow: auto;
  white-space: nowrap;
`;

const MenuContainer = styled.div`
  margin: 0 auto;
  max-width: 1400px;
`;

const LinkContainer = styled.ul`
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex: 1 1 auto;
  height: 64px;
  line-height: 64px;

  @media (max-width: 768px) {
    height: 50px;
    line-height: 50px;
  }

  li {
    a {
      transition: background-color 0.3s;
      font-size: inherit;
      color: #fff;
      display: block;
      padding: 0 20px;
      text-decoration: none;

      @media (max-width: 768px) {
        padding: 0 10px;
      }
    }
    a:hover,
    .navLink-active {
      background-color: rgba(0, 0, 0, 0.1);
    }
  }
`;

const Layout: React.FC<{ children: any }> = ({ children }) => (
  <>
    <MenuWrapper>
      <MenuContainer style={{ display: 'flex' }}>
        <LinkContainer>
          <li>
            <NavLink exact to="/" activeClassName="navLink-active">
              Home
            </NavLink>
          </li>
        </LinkContainer>
        <LinkContainer style={{ flex: '0' }}>
          <li>
            <a href="#test">Help?</a>
          </li>
        </LinkContainer>
      </MenuContainer>
    </MenuWrapper>
    <LayoutContainer>
      <div>{children}</div>
    </LayoutContainer>
  </>
);

export default Layout;
