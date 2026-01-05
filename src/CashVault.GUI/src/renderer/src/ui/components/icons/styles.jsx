/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

export const IconContainer = styled.span`
  box-sizing: border-box;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  max-height: ${(p) => `calc(${p.h} + 8px)`};
  max-width: ${(p) => `calc(${p.w} + 8px)`};

  border-radius: 0.25rem;
  color: ${(p) => p.color};
  padding: 4px;

  & svg {
    display: block;
    width: ${(p) => p.w};
    height: ${(p) => p.h};
  }
`
