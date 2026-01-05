/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import styled from '@emotion/styled'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'

import { useEffect, useState } from 'react'

const Container = styled.div`
  display: flex;
  gap: 0.5rem;

  & .divider {
    display: flex;
    gap: 0.125rem;
    padding-top: 2rem;

    & span {
      width: 3px;
      height: 3px;
      border-radius: 50%;
      background-color: black;
    }
  }
`

const Page = styled.div`
  padding: 0 1.375rem;
  border-radius: 10px;
  background-color: ${(p) => (p.current ? 'var(--primary-dark)' : 'transparent')};
  color: ${(p) => (p.current ? 'var(--primary-light)' : '#6A6A6A')};
  transition: all 0.2s ease;

  font-weight: 500;
  font-size: 1.75rem;
  line-height: 3.25rem;
  text-align: center;

  ${(p) => {
    if (!p.current) {
      return `
        &:active {
          background-color: white;
        }
      `
    }

    return `
      &:active {
        opacity: 0.8;
      }
    `
  }}
`

const PreviousNext = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: var(--text-medium);
  border-radius: 10px;
  padding: 0 0.375rem;

  ${(p) => {
    if (!p.active) {
      return `background-color: var(--disabled-bg);`
    }

    return `
      &:active {
        opacity: 0.8;
      }`
  }}
`

function getPagination(currentPage, totalPages, maxVisiblePages) {
  if (totalPages <= maxVisiblePages) {
    return Array.from({ length: totalPages }, (_, i) => i + 1)
  }

  let pages = []

  if (currentPage <= Math.floor(totalPages / 2)) {
    if (currentPage === 1) {
      pages.push(1)
      pages.push(2)
      pages.push(3)
      pages.push('x')
    } else {
      pages.push(currentPage - 1)
      pages.push(currentPage)
      pages.push(currentPage + 1)
      pages.push('x')
    }

    pages.push(totalPages - 2)
    pages.push(totalPages - 1)
    pages.push(totalPages)
  }

  if (currentPage > Math.floor(totalPages / 2)) {
    pages.push(1)
    pages.push(2)
    pages.push(3)
    pages.push('x')

    if (currentPage === totalPages) {
      pages.push(totalPages - 2)
      pages.push(totalPages - 1)
      pages.push(totalPages)
    } else {
      pages.push(currentPage - 1)
      pages.push(currentPage)
      pages.push(currentPage + 1)
    }
  }

  return pages
}

const Pagination = ({
  className,
  totalPages = 12,
  currentPage = 2,
  maxVisiblePages = 6,
  //onPrevious = () => {},
  //onNext = () => {},
  onPage = () => {}
}) => {
  const [pages, setPages] = useState([])

  useEffect(() => {
    setPages(getPagination(currentPage, totalPages, maxVisiblePages))
  }, [currentPage, totalPages])

  return (
    <Container className={className}>
      <PreviousNext
        onClick={() => {
          if (currentPage > 1) onPage(currentPage - 1)
        }}
        active={currentPage > 1}
      >
        <IconLeftHalfArrow size="m" color="var(--primary-dark)" />
      </PreviousNext>

      {pages?.map((x, ix) => {
        if (x !== 'x') {
          return (
            <Page key={ix} current={x === currentPage} onClick={() => onPage(x)}>
              {x}
            </Page>
          )
        }

        return (
          <div key={ix} className="divider">
            <span className="dot" />
            <span className="dot" />
            <span className="dot" />
          </div>
        )
      })}

      <PreviousNext
        onClick={() => {
          if (currentPage < totalPages - 1) onPage(currentPage + 1)
        }}
        active={currentPage < totalPages - 1}
      >
        <IconRightHalfArrow size="m" color="var(--primary-dark)" />
      </PreviousNext>
    </Container>
  )
}

export default Pagination
